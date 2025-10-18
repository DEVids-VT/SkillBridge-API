using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.UserProjectAssignment;

/// <summary>
/// Interface for the user project assignment service
/// </summary>
public interface IUserProjectAssignmentService
{
    /// <summary>
    /// Claims a project assignment for a user
    /// </summary>
    /// <param name="userId">The ID of the user claiming the project</param>
    /// <param name="request">The claim project request</param>
    /// <returns>The claimed project assignment</returns>
    Task<UserProjectAssignmentResponse> ClaimProjectAsync(string userId, ClaimProjectRequest request);
    
    /// <summary>
    /// Gets all project assignments claimed by a user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>A list of project assignments claimed by the user</returns>
    Task<IEnumerable<UserProjectAssignmentResponse>> GetUserProjectsAsync(string userId);

    /// <summary>
    /// Marks a project assignment as completed by a user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="request">The complete project request</param>
    /// <returns>The updated user project assignment</returns>
    Task<UserProjectAssignmentResponse> CompleteProjectAsync(string userId, CompleteUserProjectAssignmentRequest request);
}