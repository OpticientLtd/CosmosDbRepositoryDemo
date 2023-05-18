namespace CosmosDbRepositoryDemo.Domain.Models;
public class Employee : Item
{
    public string Name { get; set; }
    public Department Department { get; set; }
    public int Salary { get; set; }

}
