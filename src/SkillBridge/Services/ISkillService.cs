using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services;

/// <summary>
/// Interface for the skill service
/// </summary>
public interface ISkillService
{
    /// <summary>
    /// Creates a new skill
    /// </summary>
    /// <param name="request">The skill creation request</param>
    /// <returns>The created skill</returns>
    Task<SkillResponse> CreateAsync(CreateSkillRequest request);
    
    /// <summary>
    /// Gets a skill by ID
    /// </summary>
    /// <param name="id">The skill ID</param>
    /// <returns>The skill if found</returns>
    Task<SkillResponse?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Gets all skills
    /// </summary>
    /// <returns>A list of all skills</returns>
    Task<IEnumerable<SkillResponse>> GetAllAsync();
    
    /// <summary>
    /// Updates a skill
    /// </summary>
    /// <param name="id">The skill ID</param>
    /// <param name="request">The skill update request</param>
    /// <returns>The updated skill</returns>
    Task<SkillResponse?> UpdateAsync(Guid id, UpdateSkillRequest request);
    
    /// <summary>
    /// Deletes a skill
    /// </summary>
    /// <param name="id">The skill ID</param>
    /// <returns>True if skill was deleted, false otherwise</returns>
    Task<bool> DeleteAsync(Guid id);
}
