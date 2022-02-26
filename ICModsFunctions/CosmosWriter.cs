using ApplicationCore.Model.Cosmos;
using ApplicationCore.Model.InnerCore;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;

namespace ICModsFunctions
{
	public static class CosmosWriter
    {
        [FunctionName(nameof(WriteCosmosStat))]
        public static void WriteCosmosStat(
            [ActivityTrigger] InnerCoreModDescription description,
            [CosmosDB(
                databaseName: "ICModsStatistics",
                collectionName: "Main",
                ConnectionStringSetting = "CosmosDBConnection",
                PartitionKey = "/ModId",
                CreateIfNotExists = true
            ) ]out dynamic document,
            ILogger log)
        {
            var item = new ModStatItem
            {
                ModId = description.Id,
                StatTime = DateTime.UtcNow,
                DownloadsCount = description.Downloads,
                LikesCount = description.Likes,
                ModVersion = description.VersionName
            };
            document = item;
        }
    }
}
