using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ICModsFunctions
{
    public static class UpdateModInfo
    {
        static readonly string tableName = "mods_downloads";

        [FunctionName("UpdateModInfo")]
        public static int Run([ActivityTrigger] string modID, ILogger log)
        {
            if (Int32.TryParse(modID, out int downloadsCount)) {
                log.LogInformation($"{modID}");
                string downloads = Mineprogramming.GetDownloads(modID);

                if (String.IsNullOrEmpty(downloads)) return 0;
                log.LogInformation($"{modID} for scan");
                makeStatEntry(modID, downloads, log);
                return 1;
            }
            return 0;
        }

        public static string makeStatEntry(string modID, string downloads, ILogger log)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            string newStatID = String.Empty;
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var sb = new StringBuilder();
                sb.AppendLine($"INSERT into {tableName}(stat_time, mod_id, downloads)");
                sb.AppendLine("VALUES");
                log.LogInformation($"(CONVERT(smalldatetime, GETDATE()), {modID}, {downloads})");
                sb.AppendLine($"(CONVERT(smalldatetime, GETDATE()), {modID}, {downloads})");
                // sb.AppendLine("SELECT SCOPE_IDENTITY()");

                using (SqlCommand cmd = new SqlCommand(sb.ToString(), conn))
                {
                    Task.Run(() => cmd.ExecuteNonQuery()).Wait();
                }

            }
            return newStatID;
        }
    }
}
