namespace CosmosDbRepositoryDemo.Application;

using AutoMapper;

using CosmosDbRepositoryDemo.Application.DTOs;
using CosmosDbRepositoryDemo.Application.Models.Requests;
using CosmosDbRepositoryDemo.Domain.Models;

public static class AutoMapperConfigurations
{
    public static void ConfigureMappings(this IMapperConfigurationExpression configuration)
    {
        configuration.CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => int.Parse(src.Id)));

        configuration.CreateMap<EditDepartmentRequestModel, Department>();
        configuration.CreateMap<AddDepartmentRequestModel, Department>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

        configuration.CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => int.Parse(src.Id)));

        configuration.CreateMap<EditEmployeeRequestModel, Employee>();
        configuration.CreateMap<AddEmployeeRequestModel, Employee>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
    }
}
