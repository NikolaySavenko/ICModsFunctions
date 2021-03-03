using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ICModsFunctions.Model
{
    public partial class ModsDownloadsTest
    {
        public int StatId { get; set; }
        public DateTime StatTime { get; set; }
        public int ModId { get; set; }
        public int Downloads { get; set; }
    }
}
