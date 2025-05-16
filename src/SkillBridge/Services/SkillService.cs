using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services;

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
        
        var skill = _mapper.Map<Skill>(request);
        
        await _dbContext.Skills.AddAsync(skill);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Skill created successfully with ID: {SkillId}", skill.Id);
        
        return _mapper.Map<SkillResponse>(skill);
    }

    /// <summary>
    /// Gets a skill by ID
    /// </summary>
    public async Task<SkillResponse?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving skill with ID: {SkillId}", id);
        
        var skill = await _dbContext.Skills.FindAsync(id);
        
        if (skill == null)
        {
            _logger.LogWarning("Skill with ID {SkillId} not found", id);
            return null;
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
    public async Task<SkillResponse?> UpdateAsync(Guid id, UpdateSkillRequest request)
    {
        _logger.LogInformation("Updating skill with ID: {SkillId}", id);
        
        var skill = await _dbContext.Skills.FindAsync(id);
        
        if (skill == null)
        {
            _logger.LogWarning("Skill with ID {SkillId} not found", id);
            return null;
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
    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting skill with ID: {SkillId}", id);
        
        var skill = await _dbContext.Skills.FindAsync(id);
        
        if (skill == null)
        {
            _logger.LogWarning("Skill with ID {SkillId} not found", id);
            return false;
        }
        
        _dbContext.Skills.Remove(skill);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Skill deleted successfully: {SkillId}", id);
        
        return true;
    }
}
