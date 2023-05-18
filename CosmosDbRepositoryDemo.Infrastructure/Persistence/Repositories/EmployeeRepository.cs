namespace CosmosDbRepositoryDemo.Infrastructure.Persistence.Repositories;
using CosmosDbRepositoryDemo.Application.Interfaces.Persistence;
using CosmosDbRepositoryDemo.Domain.Models;
using CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;

public class EmployeeRepository : CosmosDbRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ICosmosDbContainerFactory cosmosDbContainerFactory) : base(cosmosDbContainerFactory)
    {
    }

    public override string ContainerName => "Employee";

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId)
    {
        return await base.GetItemsAsync($"SELECT * FROM c where c.Department.id=\"{departmentId}\"");
    }

    public async Task<long> GetEmployeesCountByDepartmentIdAsync(int departmentId)
    {
        return await base.GetScalarValueAsync<long>($"SELECT VALUE COUNT(1) FROM c where c.Department.id=\"{departmentId}\"");
    }
}
