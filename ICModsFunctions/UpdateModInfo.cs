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
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ICModsFunctions
{
    public static class UpdateModInfo
    {
        static readonly string descriptionAPI = "https://icmods.mineprogramming.org/api/description";
        static readonly string tableName = "mods_downloads";
        [FunctionName("UpdateModInfo")]
        public static int Run([ActivityTrigger] string input, ILogger log)
        {
            string[] data = input.Split(",");
            string downloads = getDownloadsForMod(data[1], log);

            if (!String.IsNullOrEmpty(downloads))
            {
                Task.Run(() => updateTableAsync(data[0], data[1], downloads, log)).Wait();
            } 

            return 0;
        }

        public static async Task<int> updateTableAsync(string statID, string modID, string downloads, ILogger log) {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");

            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var sb = new StringBuilder();
                sb.AppendLine($"UPDATE {tableName} SET modID_{modID} = {downloads} where stat_id = {statID}");
                
                using (SqlCommand cmd = new SqlCommand(sb.ToString(), conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return 0;
        }

        public static string getDownloadsForMod(string modID, ILogger log)
        {
            dynamic description = JsonConvert.DeserializeObject(getModDescription(modID, log));
            return description.downloads;
        }

        public static string getModDescription(string modID, ILogger log)
        {
            string requestURI = getRequestById(modID);
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
