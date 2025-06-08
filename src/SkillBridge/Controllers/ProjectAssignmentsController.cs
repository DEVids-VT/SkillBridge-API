using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.ProjectAssignment;

namespace SkillBridge.Controllers;

/// <summary>
/// Controller for project assignment operations
/// </summary>
[ApiController]
[Route("api/p")]
public class ProjectAssignmentsController : ControllerBase
{
    private readonly IProjectAssignmentService _projectAssignmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectAssignmentsController"/> class.
    /// </summary>
    /// <param name="projectAssignmentService">The project assignment service</param>
    public ProjectAssignmentsController(IProjectAssignmentService projectAssignmentService)
    {
        _projectAssignmentService = projectAssignmentService;
    }

    /// <summary>
    /// Creates a new project assignment for a company
    /// </summary>
    /// <param name="companyId">The ID of the company creating the project assignment</param>
    /// <param name="request">The project assignment creation request</param>
    /// <returns>The created project assignment</returns>
    [Authorize(Policy = "CompanyScope")]
    [HttpPost]
    [ProducesResponseType(typeof(ProjectAssignmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Guid companyId, [FromBody] CreateProjectAssignmentRequest request)
    {
        var projectAssignment = await _projectAssignmentService.CreateAsync(companyId, request);
        return CreatedAtAction(nameof(GetById), new { id = projectAssignment.Id }, projectAssignment);
    }

    /// <summary>
    /// Gets a project assignment by ID
    /// </summary>
    /// <param name="id">The project assignment ID</param>
    /// <returns>The project assignment if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectAssignmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]    public async Task<IActionResult> GetById(Guid id)
    {
        var projectAssignment = await _projectAssignmentService.GetByIdAsync(id);
        return Ok(projectAssignment);
    }

    /// <summary>
    /// Gets all project assignments
    /// </summary>
    /// <returns>A list of all project assignments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectAssignmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var projectAssignments = await _projectAssignmentService.GetAllAsync();
        return Ok(projectAssignments);
    }

    /// <summary>
    /// Gets all project assignments for a specific company
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <returns>A list of project assignments for the company</returns>
    [HttpGet("mine/{companyId}")]
    [ProducesResponseType(typeof(IEnumerable<ProjectAssignmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCompanyId(Guid companyId)
    {
        var projectAssignments = await _projectAssignmentService.GetByCompanyIdAsync(companyId);
        return Ok(projectAssignments);
    }

    /// <summary>
    /// Updates a project assignment
    /// </summary>
    /// <param name="id">The project assignment ID</param>
    /// <param name="request">The project assignment update request</param>
    /// <returns>The updated project assignment</returns>
    [Authorize(Policy = "CompanyScope")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProjectAssignmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]    
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectAssignmentRequest request)
    {
        var projectAssignment = await _projectAssignmentService.UpdateAsync(id, request);
        return Ok(projectAssignment);
    }

    /// <summary>
    /// Deletes a project assignment
    /// </summary>
    /// <param name="id">The project assignment ID</param>
    /// <returns>No content if deleted successfully</returns>
    [Authorize(Policy = "CompanyScope")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]    
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectAssignmentService.DeleteAsync(id);
        return NoContent();
    }
}
