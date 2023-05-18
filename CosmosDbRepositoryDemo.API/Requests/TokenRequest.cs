namespace CosmosDbRepositoryDemo.API.Requests;

using System.ComponentModel.DataAnnotations;

public class TokenRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
