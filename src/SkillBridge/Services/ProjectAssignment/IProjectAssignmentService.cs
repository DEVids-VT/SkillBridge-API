using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Models.Specifications;

namespace SkillBridge.Services.ProjectAssignment;

/// <summary>
/// Interface for the project assignment service
/// </summary>
public interface IProjectAssignmentService
{
    /// <summary>
    /// Creates a new project assignment for a company
    /// </summary>
    /// <param name="companyId">The ID of the company creating the project assignment</param>
    /// <param name="request">The project assignment creation request</param>
    /// <returns>The created project assignment</returns>
    /// <exception cref="ArgumentException">Thrown when company or skills don't exist</exception>
    Task<ProjectAssignmentResponse> CreateAsync(Guid companyId, CreateProjectAssignmentRequest request);

    /// <summary>
    /// Gets a project assignment by ID
    /// </summary>
    /// <param name="id">The project assignment ID</param>
    /// <returns>The project assignment if found</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment is not found</exception>
    Task<ProjectAssignmentResponse> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all project assignments
    /// </summary>
    /// <returns>A list of all project assignments</returns>
    Task<IEnumerable<ProjectAssignmentResponse>> GetAllAsync();

    /// <summary>
    /// Gets all project assignments for a specific company
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <returns>A list of project assignments for the company</returns>
    Task<IEnumerable<ProjectAssignmentResponse>> GetByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Updates a project assignment
    /// </summary>
    /// <param name="id">The project assignment ID</param>
    /// <param name="request">The project assignment update request</param>
    /// <returns>The updated project assignment</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment is not found</exception>
    /// <exception cref="ArgumentException">Thrown when skills don't exist</exception>
    Task<ProjectAssignmentResponse> UpdateAsync(Guid id, UpdateProjectAssignmentRequest request);

    /// <summary>
    /// Deletes a project assignment
    /// </summary>
    /// <param name="id">The project assignment ID</param>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment is not found</exception>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Creates a new task for a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="request">The task creation request</param>
    /// <returns>The created task</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment is not found</exception>
    Task<AssignmentTaskResponse> CreateTaskAsync(Guid projectId, CreateAssignmentTaskRequest request);

    /// <summary>
    /// Gets all tasks for a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <returns>A list of tasks for the project assignment</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment is not found</exception>
    Task<IEnumerable<AssignmentTaskResponse>> GetTasksAsync(Guid projectId);

    /// <summary>
    /// Gets a specific task from a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="taskId">The ID of the task</param>
    /// <returns>The task if found</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment or task is not found</exception>
    Task<AssignmentTaskResponse> GetTaskByIdAsync(Guid projectId, Guid taskId);

    /// <summary>
    /// Updates a specific task in a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="taskId">The ID of the task</param>
    /// <param name="request">The task update request</param>
    /// <returns>The updated task</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment or task is not found</exception>
    Task<AssignmentTaskResponse> UpdateTaskAsync(Guid projectId, Guid taskId, UpdateAssignmentTaskRequest request);

    /// <summary>
    /// Deletes a specific task from a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="taskId">The ID of the task</param>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment or task is not found</exception>
    Task DeleteTaskAsync(Guid projectId, Guid taskId);

    /// <summary>
    /// Searches and retrieves all project assignments.
    /// </summary>

    // Change when PagedList is implemented
    // public async Task<PagedList<ProjectAssignmentResponse>> SearchAsync
    Task<IEnumerable<ProjectAssignmentResponse>> SearchAsync(Specification<Models.Entities.ProjectAssignment> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    /// Updates a specific task in a project assignment
    /// </summary>
    /// <param name="projectId">The ID of the project assignment</param>
    /// <param name="taskId">The ID of the task</param>
    /// <returns>The completed/uncompleted task</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when project assignment or task is not found</exception>
    Task<AssignmentTaskResponse> CompleteTaskAsync(Guid projectId, Guid taskId);
}
