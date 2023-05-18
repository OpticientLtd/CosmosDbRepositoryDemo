namespace CosmosDbRepositoryDemo.Application.Models.Requests;
using System.ComponentModel.DataAnnotations;

public class EditEmployeeRequestModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public AddDepartmentRequestModel Department { get; set; }
    [Required]
    public int Salary { get; set; }
}
