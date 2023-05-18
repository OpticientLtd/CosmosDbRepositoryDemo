
namespace CosmosDbRepositoryDemo.API.Controllers;

using AutoMapper;

using CosmosDbRepositoryDemo.API.Handlers;
using CosmosDbRepositoryDemo.Application.DTOs;
using CosmosDbRepositoryDemo.Application.Interfaces.Persistence;
using CosmosDbRepositoryDemo.Application.Models.Requests;
using CosmosDbRepositoryDemo.Domain.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public EmployeesController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Gives Employee record based on given identifier. This API uses Basic Authentication.
    /// </summary>
    /// <param name="id">Employee Identifier</param>
    /// <returns>Employee DTO</returns>

    [HttpGet("{id}", Name = nameof(GetEmployeeByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeByIdAsync(int id)
    {
        var item = await _employeeRepository.GetByIdAsync(id.ToString());
        if (item != null)
        {
            return Ok(_mapper.Map<Employee, EmployeeDto>(item));
        }

        return NotFound($"Employee with Id = {id} doesn't exist.");
    }

    /// <summary>
    /// Gives list of All Employee records.  This API uses Basic Authentication.
    /// </summary>
    /// <returns>List of Employee DTO</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        return Ok(_mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeDto>>(await _employeeRepository.GetItemsAsync()));
    }

    /// <summary>
    /// Gives list of All Employee records for given Department identifier.  This API uses Basic Authentication.
    /// </summary>
    /// <param name="departmentId">Department Identifier</param>
    /// <returns>List of Employee DTO</returns>
    [HttpGet("ByDepartment/{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartmentIdAsync(int departmentId)
    {
        return Ok(_mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeDto>>(await _employeeRepository.GetEmployeesByDepartmentIdAsync(departmentId)));
    }

    /// <summary>
    /// Creates Employee record. This API uses Basic Authentication.
    /// </summary>
    /// <param name="requestModel">Data to be inserted</param>
    /// <returns>Created Employee DTO</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<EmployeeDto>> CreateAsync(AddEmployeeRequestModel requestModel)
    {
        if (!await DepartmentExists(requestModel.Department))
        {
            return BadRequest($"Department with Id = {requestModel.Department.Id} and Name = {requestModel.Department.Name} doesn't exist.");
        }

        var createdItem = await _employeeRepository.AddAsync(_mapper.Map<AddEmployeeRequestModel, Employee>(requestModel));
        if (createdItem != null)
        {
            return CreatedAtRoute(nameof(GetEmployeeByIdAsync), new { createdItem.Id }, _mapper.Map<Employee, EmployeeDto>(createdItem));
        }

        return BadRequest($"Employee with Id = {requestModel.Id} already exists.");

    }

    /// <summary>
    /// Updates Employee record for given Identifier. This API uses Basic Authentication.
    /// </summary>
    /// <param name="id">Employee Identifier</param>
    /// <param name="requestModel">Data to be updated</param>
    /// <returns>Updated Employee DTO</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<EmployeeDto>> Update(int id, EditEmployeeRequestModel requestModel)
    {
        if (!await DepartmentExists(requestModel.Department))
        {
            return BadRequest($"Department with Id = {requestModel.Department.Id} and Name = {requestModel.Department.Name} doesn't exist.");
        }

        var updatedItem = await _employeeRepository.UpdateAsync(id.ToString(), _mapper.Map<EditEmployeeRequestModel, Employee>(requestModel));
        if (updatedItem != null)
        {
            return Ok(_mapper.Map<Employee, EmployeeDto>(updatedItem));
        }

        return NotFound($"Employee with Id = {id} doesn't exist.");
    }

    /// <summary>
    /// Deletes Employee record for given identifier. This API uses Basic Authentication.
    /// </summary>
    /// <param name="id">Department Identifier</param>
    /// <returns>OK for success, NotFound otherwise.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = "JwtAuthenticationScheme")]
    public async Task<ActionResult> Delete(int id)
    {
        var isDeleted = await _employeeRepository.DeleteAsync(id.ToString());
        if (isDeleted)
        {
            return Ok("Employee is deleted successfully!");
        }

        return NotFound($"Employee with Id = {id} doesn't exist.");
    }

    private async Task<bool> DepartmentExists(AddDepartmentRequestModel requestModel)
    {
        var department = await _departmentRepository.GetByIdAsync(requestModel.Id.ToString());
        return (department?.Name.Equals(requestModel.Name, StringComparison.OrdinalIgnoreCase)).GetValueOrDefault(false);
    }
}
