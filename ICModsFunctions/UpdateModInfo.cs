using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace ICModsFunctions
{
    public static class UpdateModInfo
    {
        static readonly string descriptionAPI = "https://icmods.mineprogramming.org/api/description";
        [FunctionName("UpdateModInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,

            ILogger log)
        {
            string modID = req.Query["id"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            modID = modID ?? data?.name;

            int? downloads = getDownloadsForMod(modID, log);

            string responseMessage = downloads == null
                ? "Fail!"
                : $"Mod {modID}\nDownloads: {downloads}";
            // new BadRequestObjectResult()
            return new OkObjectResult(responseMessage);
        }

        public static int? getDownloadsForMod(string modID, ILogger log)
        {
            log.LogInformation($"Getting downloads for: {modID}");
            dynamic description = JsonConvert.DeserializeObject(getModDescription(modID, log));
            return description.downloads;
        }

        public static string getModDescription(string modID, ILogger log)
        {
            log.LogInformation($"Getting description for: {modID}");
            string requestURI = getRequestById(modID);
            log.LogInformation($"URI {requestURI}");
            var request = WebRequest.Create(requestURI);
            request.Credentials = CredentialCache.DefaultCredentials;

            var response = request.GetResponse();
            string description;

            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                description = reader.ReadToEnd();
                response.Close();
                reader.Close();
            }
            return description;
        }

        public static string getRequestById(string modID)
        {
            return $"{descriptionAPI}?id={modID}";
        }
    }
}
