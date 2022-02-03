using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ICModsFunctions.Model.Cosmos
{
    internal class ICModsCosmosClientHelper : IDisposable
    {
        // Logger
        private readonly ILogger _logger;

        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("ConnectionStrings:EndPointUri");

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("ConnectionStrings:PrimaryKey");

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        private ICModsCosmosClientHelper(ILogger log)
        {
            _logger = log;
        }
        
        // Looks like a factory method
        public static async Task<ICModsCosmosClientHelper> BuildHelper(ILogger log)
        {
            var helper = new ICModsCosmosClientHelper(log);
            helper.ConfigureCosmosClient();
            await helper.ConfigureDatabaseAndContainer();
            return helper;
        }

        private async Task ConfigureDatabaseAndContainer()
        {
            if (this.container == null)
            {
                if (this.database == null)
                {
                    await CreateDatabaseAsync();
                }
                await CreateContainerAsync();
            }
        }

        private void ConfigureCosmosClient()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions
            {
                ApplicationName = "ICModsFunctions",
                AllowBulkExecution = true
            });
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            var databaseId = Environment.GetEnvironmentVariable("DatabaseId");
            if (!string.IsNullOrEmpty(databaseId))
            {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
                _logger.LogInformation("Database with databaseId: {0} was initialized\n", databaseId);
            }
            else
            {
                _logger.LogError("Database with databaseId: {0} cannot be initialized\n", databaseId);
            }
        }

        private async Task CreateContainerAsync()
        {
            // Create a new container
            var containerId = Environment.GetEnvironmentVariable("ContainerId");
            if (!string.IsNullOrEmpty(containerId))
            {
                this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/ModId");
                _logger.LogInformation("Container in database with containerId: {0} was initialized\n", containerId);
            } else
            {
                _logger.LogError("Container in database with containerId: {0} cannot be initialized\n", containerId);
            }
        }


        public async Task AddItemToContainerAsync(ModStatItem statItem)
        {
            try
            {
                ItemResponse<ModStatItem> itemResponse = await this.container.CreateItemAsync<ModStatItem>(statItem);
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public void Dispose()
        {
            ((IDisposable)cosmosClient).Dispose();
        }
    }
}
