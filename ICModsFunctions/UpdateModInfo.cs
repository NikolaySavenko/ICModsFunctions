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
using System.Data.SqlClient;
using System.Text;

namespace ICModsFunctions
{
    public static class UpdateModInfo
    {
        static readonly string descriptionAPI = "https://icmods.mineprogramming.org/api/description";
        static readonly string tableName = "mods_downloads";
        [FunctionName("UpdateModInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string responseMessage;
            string modID = req.Query["id"];
            string statID = req.Query["statID"];
            log.LogInformation(req.Query["statID"]);
            string downloads = getDownloadsForMod(modID, log);

            if (!String.IsNullOrEmpty(downloads))
            {
                log.LogInformation($"try updateTable()");
                await updateTableAsync(statID, modID, downloads, log);
                log.LogInformation($"after updateTable()");
                responseMessage = "0";
            } else responseMessage = "1";
            
            
            

            return new OkObjectResult(responseMessage);
        }

        public static async Task<int> updateTableAsync(string statID, string modID, string downloads, ILogger log) {
            log.LogInformation($"updateTable()");
            var str = Environment.GetEnvironmentVariable("sqldb_connection");

            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var sb = new StringBuilder();
                sb.AppendLine($"UPDATE {tableName} SET modID_{modID} = {downloads} where stat_id = {statID}");
                
                log.LogInformation($"sqlCommand id {sb}");
                using (SqlCommand cmd = new SqlCommand(sb.ToString(), conn))
                {
                    // Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteNonQueryAsync();
                    log.LogInformation($"{rows} rows were updated");
                    
                }
            }
            log.LogInformation($"`finished updateTable()");
            return 0;
        }

        public static string getDownloadsForMod(string modID, ILogger log)
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
