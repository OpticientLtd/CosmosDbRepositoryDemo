namespace CosmosDbRepositoryDemo.Infrastructure.Persistence;

using CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;

using Microsoft.Azure.Cosmos;

public class CosmosDbContainer : ICosmosDbContainer
{
    public Container Container { get; }

    public CosmosDbContainer(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        this.Container = cosmosClient.GetContainer(databaseName, containerName);
    }
}
