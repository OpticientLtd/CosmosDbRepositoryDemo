namespace CosmosDbRepositoryDemo.Application.Models.Requests;
using System.ComponentModel.DataAnnotations;

public class EditDepartmentRequestModel
{
    [Required]
    public string Name { get; set; }
}
