namespace CosmosDbRepositoryDemo.Infrastructure.Persistence.Repositories;
using System;
using System.Threading.Tasks;

using CosmosDbRepositoryDemo.Application.Interfaces.Persistence;
using CosmosDbRepositoryDemo.Domain.Models;
using CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;

using Microsoft.Azure.Cosmos;

public abstract class CosmosDbRepository<T> : IRepository<T>, IContainerContext<T> where T : Item
{
    public abstract string ContainerName { get; }
    private readonly ICosmosDbContainerFactory _cosmosDbContainerFactory;
    private readonly Container _container;

    public CosmosDbRepository(ICosmosDbContainerFactory cosmosDbContainerFactory)
    {
        this._cosmosDbContainerFactory = cosmosDbContainerFactory ?? throw new ArgumentNullException(nameof(ICosmosDbContainerFactory));
        this._container = this._cosmosDbContainerFactory.GetContainer(ContainerName).Container;
    }
    public async Task<T> AddAsync(T item)
    {
        try
        {
            var response = await _container.CreateItemAsync<T>(item, ResolvePartitionKey(item.Id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }

        return null;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await this._container.DeleteItemAsync<T>(id, ResolvePartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public virtual async Task<string> GenerateId(T item)
    {
        var queryDefinition = "SELECT VALUE COUNT(1) FROM c";
        var iterator = this._container.GetItemQueryIterator<int>(queryDefinition);
        var result = await iterator.ReadNextAsync();
        var count = result.First();
        return (count + 1).ToString();
    }

    public async Task<T> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(id, ResolvePartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public virtual PartitionKey ResolvePartitionKey(string itemId) => new PartitionKey(itemId);

    public async Task<T> UpdateAsync(string id, T item)
    {
        try
        {
            item.Id = id;
            var response = await this._container.ReplaceItemAsync<T>(item, id);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<T>> GetItemsAsync()
    {
        return await GetItemsAsync("SELECT * FROM c");
    }

    protected async Task<IEnumerable<T>> GetItemsAsync(string query)
    {
        FeedIterator<T> resultSetIterator = _container.GetItemQueryIterator<T>(new QueryDefinition(query));
        List<T> results = new List<T>();
        while (resultSetIterator.HasMoreResults)
        {
            FeedResponse<T> response = await resultSetIterator.ReadNextAsync();

            results.AddRange(response.ToList());
        }

        return results;
    }

    protected async Task<TValue> GetScalarValueAsync<TValue>(string query)
    {
        FeedIterator<TValue> resultSetIterator = _container.GetItemQueryIterator<TValue>(new QueryDefinition(query));
        while (resultSetIterator.HasMoreResults)
        {
            return (await resultSetIterator.ReadNextAsync()).SingleOrDefault();
        }

        return default(TValue);
    }
}
