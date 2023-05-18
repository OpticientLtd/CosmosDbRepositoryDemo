namespace CosmosDbRepositoryDemo.Application.DTOs;
public class EmployeeDto : BaseDto
{
    public string Name { get; set; }
    public DepartmentDto Department { get; set; }
    public int Salary { get; set; }
}
