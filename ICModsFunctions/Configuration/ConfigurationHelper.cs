using System;
using System.Collections.Generic;
using System.Text;

namespace ICModsFunctions.Configuration
{
    internal static class ConfigurationHelper
    {
        // dev/prod prefix
        private const string DevelopPrefix = "develop";
        private const string ProductionPrefix = "production";

        public const string AppName = "CreateICModsMetrics";

        public static string CurrentPrefix
        {
            get
            {
                return IsDevelopment ? DevelopPrefix : ProductionPrefix;
            }
        }
        
        public static bool IsDevelopment {
            get
            {
                return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            }
        }

        public static string DatabaseId { 
            get
            {
                // "CreateICModsMetrics:Settings:databaseId"
                return $"{AppName}:{CurrentPrefix}:DatabaseId";
            }
        }

        public static string ContainerId
        {
            get
            {
                // CreateICModsMetrics: Settings: containerId
                return $"{AppName}:{CurrentPrefix}:ContainerId";
            }
        }

        public static string MaxCosmosConcurrency
        {
            get
            {
                // CreateICModsMetrics: Settings: MaxCosmosConcurrency
                return $"{AppName}:{CurrentPrefix}:MaxCosmosConcurrency";
            }
        }
    }
}
