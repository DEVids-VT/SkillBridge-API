using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services;

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
    Task<ProjectAssignmentResponse> CreateAsync(Guid companyId, CreateProjectAssignmentRequest request);
    
    /// <summary>
    /// Gets a project assignment by ID
    /// </summary>
    /// <param name="id">The project assignment ID</param>
    /// <returns>The project assignment if found</returns>
    Task<ProjectAssignmentResponse?> GetByIdAsync(Guid id);
    
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
    Task<ProjectAssignmentResponse?> UpdateAsync(Guid id, UpdateProjectAssignmentRequest request);
    
    /// <summary>
    /// Deletes a project assignment
    /// </summary>
    /// <param name="id">The project assignment ID</param>
    /// <returns>True if project assignment was deleted, false otherwise</returns>
    Task<bool> DeleteAsync(Guid id);
}
