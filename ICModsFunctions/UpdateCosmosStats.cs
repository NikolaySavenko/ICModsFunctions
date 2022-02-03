using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Model.Cosmos;
using ApplicationCore.Model.InnerCore;
using Infrastructure.Data.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ICModsFunctions
{
    public class UpdateCosmosStats
    {
        private readonly IConfiguration _configuration;

        public UpdateCosmosStats(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("UpdateCosmosStats")]
        public async Task Run([ActivityTrigger] List<InnerCoreModDescription> modsDescriptions, ILogger log)
        {
            using (var helper = await ICModsCosmosClientHelper.BuildHelper(log))
            {
                var maxConcurrency = int.Parse(Environment.GetEnvironmentVariable("MaxCosmosConcurrency"));
                using(var semaphore = new SemaphoreSlim(maxConcurrency))
                {
                    var tasks = new List<Task>();
                    foreach (var description in modsDescriptions)
                    {
                        await semaphore.WaitAsync();
                        var date = DateTime.Now;
                        var item = new ModStatItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            ModId = description.Id,
                            StatTime = date,
                            DownloadsCount = description.Downloads,
                            LikesCount = description.Likes,
                            ModVersion = description.VersionName
                        };
                        var t = helper.AddItemToContainerAsync(item);
                        tasks.Add(t.ContinueWith(t => semaphore.Release()));
                    }
                    await Task.WhenAll(tasks);
                }
            }
            log.LogInformation("Db write done");
        }
    }
}