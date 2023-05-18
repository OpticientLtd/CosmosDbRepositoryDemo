namespace CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;

using Microsoft.Azure.Cosmos;

public interface ICosmosDbContainer
{
    Container Container { get; }
}
