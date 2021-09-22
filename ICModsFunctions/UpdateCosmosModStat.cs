using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ICModsFunctions.Model.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Linq;
using System;

namespace ICModsFunctions
{
    public class UpdateCosmosModStat
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRefresher _configurationRefresher;

        public UpdateCosmosModStat(IConfiguration configuration, IConfigurationRefresherProvider refresherProvider)
        {
            _configuration = configuration;
            _configurationRefresher = refresherProvider.Refreshers.First();
        }

        [FunctionName("UpdateCosmosModStat")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Make new entry" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "ModID", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **ModID** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            int modId = int.Parse(req.Query["ModID"]);

            var description = Mineprogramming.GetDescription(modId);
            var date = DateTime.Now;
            string responseMessage = $"Mod {modId}; \nDownloads {description.Downloads}";
            var item = new ModStatItem { 
                Id = String.GetHashCode(date.ToString()).ToString(),
                ModId = modId,
                StatTime = date,
                DownloadsCount = description.Downloads,
                LikesCount = description.Likes,
                ModVersion = description.VersionName
            };
            using var helper = await ICModsCosmosClientHelper.BuildHelper(_configuration, _configurationRefresher, log);
            await helper.AddItemToContainerAsync(item);

            return new OkObjectResult(responseMessage);
        }
    }
}

