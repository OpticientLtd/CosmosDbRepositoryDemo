namespace CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;

using CosmosDbRepositoryDemo.Domain.Models;

using Microsoft.Azure.Cosmos;

public interface IContainerContext<T> where T : Item

{
    string ContainerName { get; }
    Task<string> GenerateId(T item);
    PartitionKey ResolvePartitionKey(string itemId);
}
