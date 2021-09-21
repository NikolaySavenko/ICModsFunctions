using Newtonsoft.Json;
using System;

namespace ICModsFunctions.Model.Cosmos
{
    internal class ModStatItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }
        public int ModId { get; set; }
        public DateTime StatTime { get; set; }
        public int DownloadsCount { get; set; }
    }
}
