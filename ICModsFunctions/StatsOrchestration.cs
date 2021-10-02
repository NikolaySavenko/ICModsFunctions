using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;

namespace ICModsFunctions
{
	public static class StatsOrchestration
	{

		[FunctionName("StatsOrchestration")]
		public static async Task<List<InnerCoreModDescription>> RunOrchestrator(
			[OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
		{
			Uri uri = new Uri("https://icmods.mineprogramming.org/api/list?start=0&count=10000&horizon");
			var listPesponse = await context.CallHttpAsync(HttpMethod.Get, uri);
			List<InnerCoreModData> modsList = JsonConvert.DeserializeObject<List<InnerCoreModData>>(listPesponse.Content);
			var parallelsTasks = new List<Task<DurableHttpResponse>>();
			foreach (var modData in modsList)
            {
				Uri descriptionUri = new Uri($"https://icmods.mineprogramming.org/api/description?id={modData.Id}");
				parallelsTasks.Add(context.CallHttpAsync(HttpMethod.Get, descriptionUri));
            }
			var responses = await Task.WhenAll(parallelsTasks);
			var modDescriptions = new List<InnerCoreModDescription>();
			foreach(var response in responses)
            {
				var description = JsonConvert.DeserializeObject<InnerCoreModDescription>(response.Content);
				modDescriptions.Add(description);
			}
			await context.CallActivityAsync("UpdateCosmosStats", modDescriptions);
			return modDescriptions;
		}


		[FunctionName("StatsOrchestration_HttpStart")]
		public static async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
			[DurableClient] IDurableOrchestrationClient starter,
			ILogger log)
		{
			// Function input comes from the request content.
			string instanceId = await starter.StartNewAsync("StatsOrchestration", null);

			log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}