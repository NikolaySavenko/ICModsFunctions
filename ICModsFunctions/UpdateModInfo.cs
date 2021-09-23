using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Linq;
using ICModsFunctions.Model.Cosmos;
using System.Threading.Tasks;

namespace ICModsFunctions
{
    public class UpdateModInfo
	{
		private readonly IConfiguration _configuration;
		private readonly IConfigurationRefresher _configurationRefresher;

		public UpdateModInfo(IConfiguration configuration, IConfigurationRefresherProvider refresherProvider)
		{
			_configuration = configuration;
			_configurationRefresher = refresherProvider.Refreshers.First();
		}

		[FunctionName("UpdateModInfo")]
		public async Task<int> RunAsync([ActivityTrigger] int modID, ILogger log)
		{
			var description = Mineprogramming.GetDescription(modID);
			var date = DateTime.Now;
			var item = new ModStatItem
			{
				Id = Guid.NewGuid().ToString(),
				ModId = modID,
				StatTime = date,
				DownloadsCount = description.Downloads,
				LikesCount = description.Likes,
				ModVersion = description.VersionName
			};
			using var helper = await ICModsCosmosClientHelper.BuildHelper(_configuration, _configurationRefresher, log);
			await helper.AddItemToContainerAsync(item);
			return 1;
		}
	}
}
