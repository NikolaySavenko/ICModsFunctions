using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ICModsFunctions.Model.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;

namespace ICModsFunctions
{
    public class UpdateCosmosStats
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRefresher _configurationRefresher;

        public UpdateCosmosStats(IConfiguration configuration, IConfigurationRefresherProvider refresherProvider)
        {
            _configuration = configuration;
            _configurationRefresher = refresherProvider.Refreshers.First();
        }

        [FunctionName("UpdateCosmosStats")]
        public async Task Run([ActivityTrigger] List<InnerCoreModDescription> modsDescriptions, ILogger log)
        {
            using (var helper = await ICModsCosmosClientHelper.BuildHelper(_configuration, _configurationRefresher, log))
            {
                using(var semaphore = new SemaphoreSlim(10))
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