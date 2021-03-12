using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ICModsFunctions.Model;

namespace ICModsFunctions
{
	public static class UpdateModInfo
	{
		[FunctionName("UpdateModInfo")]
		public static int Run([ActivityTrigger] string modID, ILogger log)
		{
			if (Int32.TryParse(modID, out int downloadsCount))
			{
				string downloads = Mineprogramming.GetDownloads(modID);
				if (String.IsNullOrEmpty(downloads)) return 0;

				MakeStatEntry(modID, downloads, log);
				return 1;
			}
			return 0;
		}

		public static string MakeStatEntry(string modID, string downloads, ILogger log)
		{
			using (var dbContext = new ICModsStatisticsDatabaseContext())
			{
				dbContext.ModsDownloads.Add(new ModsDownloads
				{
					StatTime = DateTime.Now,
					Downloads = Int32.Parse(downloads),
					ModId = Int32.Parse(modID)
				});
				log.LogInformation($"invoked for {modID}");
				dbContext.SaveChanges();
			}
			return "";
		}
	}
}
