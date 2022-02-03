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
using Microsoft.Extensions.Configuration;
using System.Linq;
using System;
using ApplicationCore;
using ApplicationCore.Model.Cosmos;
using Infrastructure.Data.Cosmos;

namespace ICModsFunctions
{
    public class UpdateCosmosModStat
    {
        private readonly IConfiguration _configuration;

        public UpdateCosmosModStat(IConfiguration configuration)
        {
            _configuration = configuration;
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
                Id = Guid.NewGuid().ToString(),
                ModId = modId,
                StatTime = date,
                DownloadsCount = description.Downloads,
                LikesCount = description.Likes,
                ModVersion = description.VersionName
            };
            using var helper = await ICModsCosmosClientHelper.BuildHelper(log);
            await helper.AddItemToContainerAsync(item);

            return new OkObjectResult(responseMessage);
        }
    }
}

