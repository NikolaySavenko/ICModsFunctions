using Newtonsoft.Json;
using System;

namespace ApplicationCore.Model.Cosmos
{
    public class ModStatItem
    {
        public int ModId { get; set; }
        public DateTime StatTime { get; set; }
        public int DownloadsCount { get; set; }
        public int LikesCount { get; set; }
        public string ModVersion {  get; set; }
    }
}
