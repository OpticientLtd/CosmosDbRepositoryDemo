namespace CosmosDbRepositoryDemo.Domain.Models;

using Newtonsoft.Json;

public abstract class Item
{
    [JsonProperty("id")]
    public string Id { get; set; }
}
