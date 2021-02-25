using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ICModsFunctions
{
    public static class StatsOrchestration
    {

        [FunctionName("StatsOrchestration")]
        public static async Task<int> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            if (!context.IsReplaying) {
                var parallelsTasks = new List<Task<int>>();
                log.LogInformation("START Orchestration");
                foreach (var mod in Mineprogramming.GetExistingMods()) {
                    log.LogInformation($"invoked for {mod.Id}");
                    Task<int> task = context.CallActivityAsync<int>("UpdateModInfo", mod.Id.ToString());
                    parallelsTasks.Add(task);
                }
                await Task.WhenAll(parallelsTasks);
                log.LogInformation("Finished Orchestration");
            }
            return 0;
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