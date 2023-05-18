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
public class DepartmentsController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public DepartmentsController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    /// <summary>
    /// Gives Department record based on given identifier. This API uses API Key Authentication.
    /// </summary>
    /// <param name="id">Department Identifier</param>
    /// <returns>Department DTO</returns>
    [HttpGet("{id}", Name = nameof(GetDepartmentByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<DepartmentDto>> GetDepartmentByIdAsync(int id)
    {
        var item = await _departmentRepository.GetByIdAsync(id.ToString());
        if (item != null)
        {
            return Ok(_mapper.Map<Department, DepartmentDto>(item));
        }

        return NotFound($"Department with Id = {id} doesn't exist.");
    }

    /// <summary>
    /// Gives list of All Department records. This API uses API Key Authentication.
    /// </summary>
    /// <returns>List of Department DTO</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
    {
        return Ok(_mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentDto>>(await _departmentRepository.GetItemsAsync()));
    }

    /// <summary>
    /// Creates Department record. This API uses API Key Authentication.
    /// </summary>
    /// <param name="requestModel">Data to be inserted</param>
    /// <returns>Created Department DTO</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<DepartmentDto>> CreateAsync(AddDepartmentRequestModel requestModel)
    {
        var createdItem = await _departmentRepository.AddAsync(_mapper.Map<AddDepartmentRequestModel, Department>(requestModel));
        if (createdItem != null)
        {
            return CreatedAtRoute(nameof(GetDepartmentByIdAsync), new { createdItem.Id }, _mapper.Map<Department, DepartmentDto>(createdItem));
        }

        return BadRequest($"Department with Id = {requestModel.Id} already exists.");

    }

    /// <summary>
    /// Updates Department record for given Identifier. This API uses API Key Authentication.
    /// </summary>
    /// <param name="id">Department Identifier</param>
    /// <param name="requestModel">Data to be updated</param>
    /// <returns>Updated Department DTO</returns>

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.SchemeName)]
    public async Task<ActionResult<DepartmentDto>> Update(int id, EditDepartmentRequestModel requestModel)
    {
        var updatedItem = await _departmentRepository.UpdateAsync(id.ToString(), _mapper.Map<EditDepartmentRequestModel, Department>(requestModel));
        if (updatedItem != null)
        {
            return Ok(_mapper.Map<Department, DepartmentDto>(updatedItem));
        }

        return NotFound($"Department with Id = {id} doesn't exist.");
    }

    /// <summary>
    /// Deletes Department record for given identifier. This API uses JWT Authentication.
    /// </summary>
    /// <param name="id">Department Identifier</param>
    /// <returns>OK for success, NotFound otherwise.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(AuthenticationSchemes = "JwtAuthenticationScheme")]
    public async Task<ActionResult> Delete(int id)
    {
        if (await _employeeRepository.GetEmployeesCountByDepartmentIdAsync(id) > 0)
        {
            return BadRequest($"Department is linked with Employees, so can not be deleted.");
        }

        var isDeleted = await _departmentRepository.DeleteAsync(id.ToString());

        if (isDeleted)
        {
            return Ok("Department is deleted successfully!");
        }

        return NotFound($"Department with Id = {id} doesn't exist.");
    }
}
