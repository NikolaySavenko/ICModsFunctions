using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICModsFunctions
{
    public class InnerCoreModData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("horizon_optimized")]
        public int HorizonOptimized { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("version_name")]
        public string VersionName { get; set; }

        [JsonProperty("last_update")]
        public string LastUpdate { get; set; }

        [JsonProperty("vip")]
        public int Vip { get; set; }

        [JsonProperty("pack")]
        public int Pack { get; set; }

        [JsonProperty("multiplayer")]
        public int Multiplayer { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("likes")]
        public int Likes { get; set; }

        [JsonProperty("dislikes")]
        public int Dislikes { get; set; }

        [JsonProperty("liked")]
        public int Liked { get; set; }

        [JsonProperty("disliked")]
        public int Disliked { get; set; }
    }
}
