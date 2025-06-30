using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.Skill;

/// <summary>
/// Implementation of the skill service
/// </summary>
public class SkillService : ISkillService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<SkillService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillService"/> class.
    /// </summary>
    public SkillService(AppDbContext dbContext, IMapper mapper, ILogger<SkillService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new skill
    /// </summary>
    public async Task<SkillResponse> CreateAsync(CreateSkillRequest request)
    {
        _logger.LogInformation("Creating new skill with name: {SkillName}", request.Name);
        
        var skill = _mapper.Map<Models.Entities.Skill>(request);
        
        await _dbContext.Skills.AddAsync(skill);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Skill created successfully with ID: {SkillId}", skill.Id);
        
        return _mapper.Map<SkillResponse>(skill);
    }

    /// <summary>
    /// Gets a skill by ID
    /// </summary>
    public async Task<SkillResponse> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving skill with ID: {SkillId}", id);
        
        var skill = await _dbContext.Skills.FindAsync(id);
        
        if (skill == null)
        {
            _logger.LogWarning("Skill with ID {SkillId} not found", id);
            throw new EntityNotFoundException(nameof(Models.Entities.Skill), id);
        }
        
        _logger.LogInformation("Skill found: {SkillName}", skill.Name);
        
        return _mapper.Map<SkillResponse>(skill);
    }

    /// <summary>
    /// Gets all skills
    /// </summary>
    public async Task<IEnumerable<SkillResponse>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all skills");
        
        var skills = await _dbContext.Skills.ToListAsync();
        
        _logger.LogInformation("Retrieved {SkillCount} skills", skills.Count);
        
        return _mapper.Map<IEnumerable<SkillResponse>>(skills);
    }

    /// <summary>
    /// Updates a skill
    /// </summary>
    public async Task<SkillResponse> UpdateAsync(Guid id, UpdateSkillRequest request)
    {
        _logger.LogInformation("Updating skill with ID: {SkillId}", id);
        
        var skill = await _dbContext.Skills.FindAsync(id);
        
        if (skill == null)
        {
            _logger.LogWarning("Skill with ID {SkillId} not found", id);
            throw new EntityNotFoundException(nameof(Models.Entities.Skill), id);
        }
        
        _mapper.Map(request, skill);
        
        _dbContext.Skills.Update(skill);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Skill updated successfully: {SkillName}", skill.Name);
        
        return _mapper.Map<SkillResponse>(skill);
    }

    /// <summary>
    /// Deletes a skill
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting skill with ID: {SkillId}", id);
        
        var skill = await _dbContext.Skills.FindAsync(id);
        
        if (skill == null)
        {
            _logger.LogWarning("Skill with ID {SkillId} not found", id);
            throw new EntityNotFoundException(nameof(Models.Entities.Skill), id);
        }
        
        _dbContext.Skills.Remove(skill);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Skill deleted successfully: {SkillId}", id);
    }
    
    /// <summary>
    /// Validates that all skills with the given IDs exist
    /// </summary>
    /// <param name="skillIds">List of skill IDs to validate</param>
    /// <returns>List of validated skill IDs</returns>
    /// <exception cref="EntityNotFoundException">Thrown when any skill ID is not found</exception>
    public async Task<List<Guid>> ValidateSkillsExistAsync(List<Guid> skillIds)
    {
        if (skillIds == null || !skillIds.Any())
        {
            return new List<Guid>();
        }
            
        _logger.LogInformation("Validating {SkillCount} skills", skillIds.Count);
        
        var existingSkills = await _dbContext.Skills
            .Where(s => skillIds.Contains(s.Id))
            .ToListAsync();
            
        var existingSkillIds = existingSkills.Select(s => s.Id).ToList();
        var missingSkillIds = skillIds.Except(existingSkillIds).ToList();
            
        if (missingSkillIds.Any())
        {
            var missingSkillIdsString = string.Join(", ", missingSkillIds);
            _logger.LogWarning("Skills with the following IDs were not found: {SkillIds}", missingSkillIdsString);
            throw new EntityNotFoundException("Skill", missingSkillIdsString,
                $"Skills with the following IDs were not found: {missingSkillIdsString}");
        }
            
        _logger.LogInformation("All {SkillCount} skills validated successfully", skillIds.Count);
        return existingSkillIds;
    }
    
    /// <summary>
    /// Gets or creates skills by their names
    /// </summary>
    /// <param name="skillNames">List of skill names</param>
    /// <returns>List of skill IDs that exist in the database</returns>
    public async Task<List<Guid>> GetOrCreateSkillsByNameAsync(List<string> skillNames)
    {
        if (skillNames == null || !skillNames.Any())
        {
            return new List<Guid>();
        }
            
        _logger.LogInformation("Getting or creating {SkillCount} skills by name", skillNames.Count);
        
        var normalizedNames = skillNames.Select(name => name.Trim()).Where(name => !string.IsNullOrEmpty(name)).Distinct().ToList();
        
        // Find existing skills by name
        var existingSkills = await _dbContext.Skills
            .Where(s => normalizedNames.Contains(s.Name))
            .ToListAsync();
            
        var existingSkillNames = existingSkills.Select(s => s.Name).ToList();
        var missingSkillNames = normalizedNames.Except(existingSkillNames, StringComparer.OrdinalIgnoreCase).ToList();
            
        // Create missing skills
        if (missingSkillNames.Any())
        {
            _logger.LogInformation("Creating {MissingSkillCount} new skills", missingSkillNames.Count);
            
            var newSkills = missingSkillNames.Select(name => new Models.Entities.Skill
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = null
            }).ToList();
                
            await _dbContext.Skills.AddRangeAsync(newSkills);
            await _dbContext.SaveChangesAsync();
                
            existingSkills.AddRange(newSkills);
        }
            
        _logger.LogInformation("Returning {SkillCount} skill IDs", existingSkills.Count);
        return existingSkills.Select(s => s.Id).ToList();
    }
}
