﻿using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;

namespace ICModsFunctions.Model.Cosmos
{
    internal class ICModsCosmosClientHelper : IDisposable
    {
        // Cloud Config
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRefresher _configurationRefresher;

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

        public ICModsCosmosClientHelper(IConfiguration configuration, IConfigurationRefresher refresher, ILogger log)
        {
            _logger = log;
            _configuration = configuration;
            _configurationRefresher = refresher;
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions { ApplicationName = "CreateICModsMetrics" });
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            var databaseId = _configuration["CreateICModsMetrics:Settings:databaseId"];
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
            var containerId = _configuration["CreateICModsMetrics:Settings:containerId"];
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
            if (this.container == null)
            {
                if (this.database == null)
                {
                    await CreateDatabaseAsync();
                }
                await CreateContainerAsync();
            }
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<ModStatItem> itemResponse = await this.container.ReadItemAsync<ModStatItem>(statItem.Id.ToString(), new PartitionKey(statItem.ModId));
                _logger.LogInformation("Item in database with id: {0} already exists\n", itemResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<ModStatItem> itemResponse = await this.container.CreateItemAsync<ModStatItem>(statItem);
                _logger.LogInformation("Item in database with id: {0} was created\n", itemResponse.Resource.Id);
            }
        }

        public async Task<bool> TryRefreshConfigAsync()
        {
            return await _configurationRefresher.TryRefreshAsync();
        }

        public void Dispose()
        {
            ((IDisposable)cosmosClient).Dispose();
        }
    }
}
