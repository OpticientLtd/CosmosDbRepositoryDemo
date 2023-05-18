namespace CosmosDbRepositoryDemo.Infrastructure;

using CosmosDbRepositoryDemo.Application;
using CosmosDbRepositoryDemo.Application.Interfaces.Persistence;
using CosmosDbRepositoryDemo.Infrastructure.Configurations;
using CosmosDbRepositoryDemo.Infrastructure.Persistence;
using CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;
using CosmosDbRepositoryDemo.Infrastructure.Persistence.Repositories;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ConfigureInfrastructureServices
{
    public static IServiceCollection AddInfrastructures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(config => AutoMapperConfigurations.ConfigureMappings(config));
        services.AddCosmosDb(configuration);
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        return services;
    }
    public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
    {
        CosmosDbConfiguration cosmosDbConfig = configuration.GetSection(CosmosDbConfiguration.SectionKey).Get<CosmosDbConfiguration>();
        CosmosClient client = new CosmosClient(cosmosDbConfig.ConnectionString);
        services.AddSingleton<ICosmosDbContainerFactory>(c => new CosmosDbContainerFactory(client, cosmosDbConfig.DatabaseName, cosmosDbConfig.Containers));

        return services;
    }
}
