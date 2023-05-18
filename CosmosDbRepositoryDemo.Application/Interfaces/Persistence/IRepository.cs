namespace CosmosDbRepositoryDemo.Application.Interfaces.Persistence;
using System.Threading.Tasks;

using CosmosDbRepositoryDemo.Domain.Models;

public interface IRepository<T>
    where T : Item

{
    Task<T> GetByIdAsync(string id);
    Task<T> AddAsync(T item);
    Task<T> UpdateAsync(string id, T item);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<T>> GetItemsAsync();
}
