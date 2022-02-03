using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ICModsFunctions.Model.Cosmos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace ICModsFunctions
{
    public class ManualUpdateCosmosModStat {

        private readonly IConfiguration _configuration;

        public ManualUpdateCosmosModStat(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("ManualUpdateCosmosModStat")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Make new entry" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "ModID", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **ModID** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            int modId = int.Parse(req.Query["ModID"]);

            var description = Mineprogramming.GetDescription(modId);
            var date = DateTime.Now;
            var item = new ModStatItem
            {
                Id = Guid.NewGuid().ToString(),
                ModId = modId,
                StatTime = date,
                DownloadsCount = description.Downloads,
                LikesCount = description.Likes,
                ModVersion = description.VersionName
            };
            using var helper = await ICModsCosmosClientHelper.BuildHelper(log);
            await helper.AddItemToContainerAsync(item);

            string responseMessage = JsonConvert.SerializeObject(item);
            return new OkObjectResult(responseMessage);
        }
    }
}

