using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ICModsFunctions
{
    public static class MakeDailyStats
    {
        static readonly string tableName = "mods_downloads";
        static readonly string updateFunctionName = "https://createicmodsmetrics.azurewebsites.net/api/UpdateModInfo";
        [FunctionName("MakeDailyStats")]
        public static void Run([TimerTrigger("* */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger MakeDailyStats executed at: {DateTime.Now}");
            var statID = makeStatEntry(log);
            updateModInfo("2", statID, log);
            log.LogInformation(statID);
        }

        public static int updateModInfo(string modID, string statID, ILogger log) {
            var request = WebRequest.Create($"{updateFunctionName}id={modID}&statID={statID}");
            request.Credentials = CredentialCache.DefaultCredentials;
            log.LogInformation($"{updateFunctionName}?id={modID}&statID={statID}");
            
            /*string description;

            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                description = reader.ReadToEnd();
                response.Close();
                reader.Close();
            }*/
            return 0;
        }

        public static string makeStatEntry(ILogger log) {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            string newStatID = String.Empty;
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var sb = new StringBuilder();
                sb.AppendLine($"INSERT into {tableName}(stat_time)");
                sb.AppendLine("VALUES");
                sb.AppendLine("(CONVERT(smalldatetime, GETDATE()))");
                sb.AppendLine("SELECT SCOPE_IDENTITY()");

                using (SqlCommand cmd = new SqlCommand(sb.ToString(), conn)) {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read())
                        {
                            log.LogInformation($"New Entry id is {reader[0].GetType()}");
                            newStatID = reader[0].ToString();
                        }
                    }
                }
                
            }
            return newStatID;
        }
    }
}
