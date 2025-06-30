using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.Skill;

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
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when skill is not found</exception>
    Task<SkillResponse> GetByIdAsync(Guid id);
    
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
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when skill is not found</exception>
    Task<SkillResponse> UpdateAsync(Guid id, UpdateSkillRequest request);
    
    /// <summary>
    /// Deletes a skill
    /// </summary>
    /// <param name="id">The skill ID</param>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when skill is not found</exception>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Validates that all skills with the given IDs exist
    /// </summary>
    /// <param name="skillIds">List of skill IDs to validate</param>
    /// <returns>List of validated skill IDs</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.EntityNotFoundException">Thrown when any skill ID is not found</exception>
    Task<List<Guid>> ValidateSkillsExistAsync(List<Guid> skillIds);
    
    /// <summary>
    /// Gets or creates skills by their names
    /// </summary>
    /// <param name="skillNames">List of skill names</param>
    /// <returns>List of skill IDs that exist in the database</returns>
    Task<List<Guid>> GetOrCreateSkillsByNameAsync(List<string> skillNames);
}
