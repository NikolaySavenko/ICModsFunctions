using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICModsFunctions
{

    public class InnerCoreModDescription
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("version_name")]
        public string VersionName { get; set; }

        [JsonProperty("filename")]
        public object Filename { get; set; }

        [JsonProperty("icon_full")]
        public string IconFull { get; set; }

        [JsonProperty("github")]
        public string Github { get; set; }

        [JsonProperty("rate")]
        public int Rate { get; set; }

        [JsonProperty("author")]
        public int Author { get; set; }

        [JsonProperty("downloads")]
        public int Downloads { get; set; }

        [JsonProperty("changelog")]
        public string Changelog { get; set; }

        [JsonProperty("last_update")]
        public string LastUpdate { get; set; }

        [JsonProperty("vip")]
        public int Vip { get; set; }

        [JsonProperty("pack")]
        public int Pack { get; set; }

        [JsonProperty("enabled")]
        public int Enabled { get; set; }

        [JsonProperty("multiplayer")]
        public int Multiplayer { get; set; }

        [JsonProperty("description_full")]
        public string DescriptionFull { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("likes")]
        public int Likes { get; set; }

        [JsonProperty("dislikes")]
        public int Dislikes { get; set; }

        [JsonProperty("liked")]
        public int Liked { get; set; }

        [JsonProperty("disliked")]
        public int Disliked { get; set; }

        [JsonProperty("author_name")]
        public string AuthorName { get; set; }

        [JsonProperty("dependencies")]
        public List<object> Dependencies { get; set; }

        [JsonProperty("addons")]
        public List<int> Addons { get; set; }

        [JsonProperty("horizon_optimized")]
        public bool HorizonOptimized { get; set; }
    }
}
