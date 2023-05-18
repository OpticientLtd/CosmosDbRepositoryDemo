namespace CosmosDbRepositoryDemo.Application.Models.Requests;
using System.ComponentModel.DataAnnotations;

public class AddEmployeeRequestModel : EditEmployeeRequestModel
{
    [Required]
    public int Id { get; set; }
}
