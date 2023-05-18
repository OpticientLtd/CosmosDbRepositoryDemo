namespace CosmosDbRepositoryDemo.Application.Interfaces.Persistence;

using CosmosDbRepositoryDemo.Domain.Models;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId);
    Task<long> GetEmployeesCountByDepartmentIdAsync(int departmentId);
}
