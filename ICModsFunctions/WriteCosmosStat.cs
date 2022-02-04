using ApplicationCore.Model.Cosmos;
using ApplicationCore.Model.InnerCore;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;

namespace ICModsFunctions
{
	public static class WriteCosmosStat
    {
        [FunctionName("WriteCosmosStat")]
        public static void Run(
            [ActivityTrigger] InnerCoreModDescription description,
            [CosmosDB(
                databaseName: "ToDoItems",
                collectionName: "Items",
                ConnectionStringSetting = "CosmosDBConnection",
                PartitionKey = "/ModId",
                CreateIfNotExists = true
            ) ]out dynamic document,
            ILogger log)
        {
            var item = new ModStatItem
            {
                Id = Guid.NewGuid().ToString(),
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
