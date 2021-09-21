using System;
using Microsoft.Azure.Cosmos;

namespace ICModsFunctions.Model.Cosmos
{
    internal class ICModsCosmosClientHelper
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("EndPointUri");

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = Environment.GetEnvironmentVariable("DatabaseId");
        private string containerId = Environment.GetEnvironmentVariable("ContainerId");

    }
}
