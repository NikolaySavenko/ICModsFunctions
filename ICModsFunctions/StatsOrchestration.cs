using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;
using ApplicationCore.Model.InnerCore;
using System.Linq;

namespace ICModsFunctions
{
	public static class StatsOrchestration
	{
		[FunctionName(nameof(StartOrchestrationByHttp))]
		public static async Task<HttpResponseMessage> StartOrchestrationByHttp(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
			[DurableClient] IDurableOrchestrationClient starter,
			ILogger log)
		{
			string instanceId = await starter.StartNewAsync(nameof(RunStatisticsOrchestrator), null);
			log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
			return starter.CreateCheckStatusResponse(req, instanceId);
		}

		[FunctionName(nameof(RunStatisticsOrchestrator))]
		public static async Task RunStatisticsOrchestrator(
			[OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
		{
			var descriptions = await GetModsDescriptions(context);
			await WriteModsDescriptions(descriptions, context);
		}

		private static async Task<IEnumerable<InnerCoreModDescription>> GetModsDescriptions(IDurableOrchestrationContext context)
		{
			var mods = await GetModsContent<InnerCoreModDescription>(context);
			var requests = mods.Select(mod => context.CallHttpAsync(HttpMethod.Get, GetDescriptionUri(mod.Id)));

			var responses = await Task.WhenAll(requests);
			var descriptions = responses.Select(r => JsonConvert.DeserializeObject<InnerCoreModDescription>(r.Content));

			return descriptions;
		}

		private static async Task WriteModsDescriptions<T>(IEnumerable<T> descriptions, IDurableOrchestrationContext context)
		{
			var writeFunctionName = nameof(CosmosWriter.WriteCosmosStat);
			var activities = descriptions.Select(d => context.CallActivityAsync(writeFunctionName, d));

			await Task.WhenAll(activities);
		}

		private static async Task<TModContent[]> GetModsContent<TModContent>(IDurableOrchestrationContext context)
		{
			var apiEndpoint = new Uri("https://icmods.mineprogramming.org/api/list?start=0&count=10000&horizon");
			var apiCall = await context.CallHttpAsync(HttpMethod.Get, apiEndpoint);
			var mods = JsonConvert.DeserializeObject<TModContent[]>(apiCall.Content);
			return mods;
		}

		private static Uri GetDescriptionUri(int modId)
		{
			return new Uri($"https://icmods.mineprogramming.org/api/description?id={modId}");
		}
	}
}