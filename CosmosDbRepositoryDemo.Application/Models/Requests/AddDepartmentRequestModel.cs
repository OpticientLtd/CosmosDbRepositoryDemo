namespace CosmosDbRepositoryDemo.Application.Models.Requests;
using System.ComponentModel.DataAnnotations;

public class AddDepartmentRequestModel : EditDepartmentRequestModel
{
    [Required]
    public int Id { get; set; }
}
