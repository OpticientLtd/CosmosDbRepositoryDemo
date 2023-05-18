namespace CosmosDbRepositoryDemo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CosmosDbRepositoryDemo.Infrastructure.Configurations;
using CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;

using Microsoft.Azure.Cosmos;

public class CosmosDbContainerFactory : ICosmosDbContainerFactory
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly List<ContainerInfo> _containers;
    public CosmosDbContainerFactory(CosmosClient cosmosClient,
                                   string databaseName,
                                   List<ContainerInfo> containers)
    {
        _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        _containers = containers ?? throw new ArgumentNullException(nameof(containers));
    }

    public async Task EnsureDbSetupAsync()
    {
        var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);

        foreach (ContainerInfo container in _containers)
        {
            await database.Database.CreateContainerIfNotExistsAsync(container.Name, $"{container.PartitionKey}");
        }
    }

    public ICosmosDbContainer GetContainer(string containerName)
    {
        if (!_containers.Any(c => c.Name == containerName))
        {
            throw new ArgumentException($"Unable to find Container:{containerName}");
        }

        return new CosmosDbContainer(_cosmosClient, _databaseName, containerName);
    }
}
