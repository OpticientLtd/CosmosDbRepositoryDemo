namespace CosmosDbRepositoryDemo.Infrastructure.Persistence.Repositories;
using CosmosDbRepositoryDemo.Application.Interfaces.Persistence;
using CosmosDbRepositoryDemo.Domain.Models;
using CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;

public class DepartmentRepository : CosmosDbRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ICosmosDbContainerFactory cosmosDbContainerFactory) : base(cosmosDbContainerFactory)
    {
    }

    public override string ContainerName => "Department";
}
