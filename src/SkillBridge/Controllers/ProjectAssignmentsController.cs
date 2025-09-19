using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Models.Specifications;
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
    [Authorize(Policy = "Company")]
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
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
    [Authorize(Policy = "Company")]
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
    [Authorize(Policy = "Company")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectAssignmentService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Creates a new task for a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="request">The task creation request</param>
    /// <returns>The created task</returns>
    [Authorize(Policy = "Company")]
    [HttpPost("{projectId}/tasks")]
    [ProducesResponseType(typeof(AssignmentTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateAssignmentTaskRequest request)
    {
        var task = await _projectAssignmentService.CreateTaskAsync(projectId, request);
        return CreatedAtAction(nameof(GetTaskById), new { projectId = projectId, taskId = task.Id }, task);
    }

    /// <summary>
    /// Gets all tasks for a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <returns>A list of tasks for the project assignment</returns>
    [HttpGet("{projectId}/tasks")]
    [ProducesResponseType(typeof(IEnumerable<AssignmentTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTasks(Guid projectId)
    {
        var tasks = await _projectAssignmentService.GetTasksAsync(projectId);
        return Ok(tasks);
    }

    /// <summary>
    /// Gets a specific task from a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="taskId">The ID of the task</param>
    /// <returns>The task if found</returns>
    [HttpGet("{projectId}/tasks/{taskId}")]
    [ProducesResponseType(typeof(AssignmentTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(Guid projectId, Guid taskId)
    {
        var task = await _projectAssignmentService.GetTaskByIdAsync(projectId, taskId);
        return Ok(task);
    }

    /// <summary>
    /// Updates a specific task in a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="taskId">The ID of the task</param>
    /// <param name="request">The task update request</param>
    /// <returns>The updated task</returns>
    [Authorize(Policy = "Company")]
    [HttpPut("{projectId}/tasks/{taskId}")]
    [ProducesResponseType(typeof(AssignmentTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(Guid projectId, Guid taskId, [FromBody] UpdateAssignmentTaskRequest request)
    {
        var task = await _projectAssignmentService.UpdateTaskAsync(projectId, taskId, request);
        return Ok(task);
    }

    /// <summary>
    /// Deletes a specific task from a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="taskId">The ID of the task</param>
    /// <returns>No content if deleted successfully</returns>
    [Authorize(Policy = "Company")]
    [HttpDelete("{projectId}/tasks/{taskId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid projectId, Guid taskId)
    {
        await _projectAssignmentService.DeleteTaskAsync(projectId, taskId);
        return NoContent();
    }

    [HttpGet("search")]
    // When pagination is implemented:
    // public async Task<IPage<OrganizationDto>> SearchOrganizations(
    public async Task<IEnumerable<ProjectAssignmentResponse>> SearchOrganizations(
    [FromQuery] SearchProjectAssignmentRequest request,
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        var specification = new ProjectAssignmentTitleSpecification(request.Title)
            .And(new ProjectAssignmentLevelSpecification(request.Level))
            .And(new ProjectAssignmentDeadlineAfterSpecification(request.DeadlineAfter))
            .And(new ProjectAssignmentCompanyNameSpecification(request.CompanyName))
            .And(new ProjectAssignmentCompanySectorSpecification(request.CompanySector))
            .And(new ProjectAssignmentSkillsSpecification(request.ProjectSkills));

        var result = await _projectAssignmentService.SearchProjectAssignmentsAsync(
            specification, pageNumber, pageSize, cancellationToken);

        //return result.ToPage();
        return result;
    }
    /// <summary>
    /// Complete a specific task in a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="taskId">The ID of the task</param>
    /// <returns>The completed/uncompleted task</returns>
    [Authorize(Policy = "Candidate")]
    [HttpPatch("{projectId}/tasks/{taskId}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTask(Guid projectId, Guid taskId)
    {
        var stats = await _projectAssignmentService.CompleteTaskAsync(projectId, taskId);
        return Ok(stats);
    }
}
