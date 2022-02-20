using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace ICModsFunctions
{
	public static class StatisticsTimer
    {
        [FunctionName(nameof(MakeDailyStats))]
        public static void MakeDailyStats([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            log.LogInformation($"C# Timer trigger MakeDailyStats executed at: {DateTime.Now} on Hub {starter.TaskHubName}");
            starter.StartNewAsync(nameof(StatsOrchestration.RunStatisticsOrchestrator));
        }
    }
}
