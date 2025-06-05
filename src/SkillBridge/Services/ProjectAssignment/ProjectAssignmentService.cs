using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.ProjectAssignment;

/// <summary>
/// Implementation of the project assignment service
/// </summary>
public class ProjectAssignmentService : IProjectAssignmentService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectAssignmentService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectAssignmentService"/> class.
    /// </summary>
    public ProjectAssignmentService(AppDbContext dbContext, IMapper mapper, ILogger<ProjectAssignmentService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new project assignment for a company
    /// </summary>
    public async Task<ProjectAssignmentResponse> CreateAsync(Guid companyId, CreateProjectAssignmentRequest request)
    {
        _logger.LogInformation("Creating new project assignment for company ID: {CompanyId}", companyId);
        
        // Verify company exists
        var companyExists = await _dbContext.Companies.AnyAsync(c => c.Id == companyId);
        if (!companyExists)
        {
            _logger.LogWarning("Company with ID {CompanyId} not found", companyId);
            throw new EntityNotFoundException("Company", companyId);
        }
        
        // Verify all skills exist
        if (request.SkillIds.Any())
        {
            var existingSkillIds = await _dbContext.Skills
                .Where(s => request.SkillIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();
            
            var missingSkillIds = request.SkillIds.Except(existingSkillIds).ToList();
            if (missingSkillIds.Any())
            {
                var skillIdsString = string.Join(", ", missingSkillIds);
                _logger.LogWarning("One or more skills not found: {SkillIds}", skillIdsString);
                throw new EntityNotFoundException("Skill", skillIdsString, 
                    $"One or more skills not found: {skillIdsString}");
            }
        }
        
        // Create the project assignment
        var projectAssignment = _mapper.Map<Models.Entities.ProjectAssignment>(request);
        projectAssignment.CompanyId = companyId;
        
        await _dbContext.ProjectAssignments.AddAsync(projectAssignment);
        await _dbContext.SaveChangesAsync();
        
        // Add project skills
        if (request.SkillIds.Any())
        {
            var projectSkills = request.SkillIds.Select(skillId => new ProjectSkill
            {
                ProjectAssignmentId = projectAssignment.Id,
                SkillId = skillId
            }).ToList();
            
            await _dbContext.ProjectSkills.AddRangeAsync(projectSkills);
            await _dbContext.SaveChangesAsync();
        }
        
        _logger.LogInformation("Project assignment created successfully with ID: {ProjectAssignmentId}", projectAssignment.Id);
        
        // Return the full project assignment with skills
        return await GetResponseWithDetailsAsync(projectAssignment.Id);
    }

    /// <summary>
    /// Gets a project assignment by ID
    /// </summary>
    public async Task<ProjectAssignmentResponse> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving project assignment with ID: {ProjectAssignmentId}", id);
        
        var projectAssignment = await _dbContext.ProjectAssignments
            .Include(p => p.Company)
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (projectAssignment == null)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", id);
            throw new EntityNotFoundException("ProjectAssignment", id);
        }
        
        return MapToResponse(projectAssignment);
    }

    /// <summary>
    /// Gets all project assignments
    /// </summary>
    public async Task<IEnumerable<ProjectAssignmentResponse>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all project assignments");
        
        var projectAssignments = await _dbContext.ProjectAssignments
            .Include(p => p.Company)
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .ToListAsync();
        
        _logger.LogInformation("Retrieved {ProjectAssignmentCount} project assignments", projectAssignments.Count);
        
        return projectAssignments.Select(p => MapToResponse(p));
    }

    /// <summary>
    /// Gets all project assignments for a specific company
    /// </summary>
    public async Task<IEnumerable<ProjectAssignmentResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        _logger.LogInformation("Retrieving project assignments for company ID: {CompanyId}", companyId);
        
        var projectAssignments = await _dbContext.ProjectAssignments
            .Include(p => p.Company)
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .Where(p => p.CompanyId == companyId)
            .ToListAsync();
        
        _logger.LogInformation("Retrieved {ProjectAssignmentCount} project assignments for company ID: {CompanyId}", 
            projectAssignments.Count, companyId);
        
        return projectAssignments.Select(p => MapToResponse(p));
    }

    /// <summary>
    /// Updates a project assignment
    /// </summary>
    public async Task<ProjectAssignmentResponse> UpdateAsync(Guid id, UpdateProjectAssignmentRequest request)
    {
        _logger.LogInformation("Updating project assignment with ID: {ProjectAssignmentId}", id);
        
        // Get project assignment with related skills
        var projectAssignment = await _dbContext.ProjectAssignments
            .Include(p => p.ProjectSkills)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (projectAssignment == null)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", id);
            throw new EntityNotFoundException("ProjectAssignment", id);
        }
        
        // Update basic properties
        _mapper.Map(request, projectAssignment);
        projectAssignment.UpdatedAt = DateTime.UtcNow;
        
        // Verify all skills exist
        if (request.SkillIds.Any())
        {
            var existingSkillIds = await _dbContext.Skills
                .Where(s => request.SkillIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();
            
            var missingSkillIds = request.SkillIds.Except(existingSkillIds).ToList();
            if (missingSkillIds.Any())
            {
                var skillIdsString = string.Join(", ", missingSkillIds);
                _logger.LogWarning("One or more skills not found: {SkillIds}", skillIdsString);
                throw new EntityNotFoundException("Skill", skillIdsString, 
                    $"One or more skills not found: {skillIdsString}");
            }
        }
        
        // Update skills - remove existing and add new ones
        _dbContext.ProjectSkills.RemoveRange(projectAssignment.ProjectSkills);
        
        var projectSkills = request.SkillIds.Select(skillId => new ProjectSkill
        {
            ProjectAssignmentId = projectAssignment.Id,
            SkillId = skillId
        }).ToList();
        
        await _dbContext.ProjectSkills.AddRangeAsync(projectSkills);
        
        // Save all changes
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Project assignment updated successfully: {ProjectAssignmentId}", id);
        
        // Return the full project assignment with skills
        return await GetResponseWithDetailsAsync(projectAssignment.Id);
    }

    /// <summary>
    /// Deletes a project assignment
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting project assignment with ID: {ProjectAssignmentId}", id);
        
        var projectAssignment = await _dbContext.ProjectAssignments
            .Include(p => p.ProjectSkills)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (projectAssignment == null)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", id);
            throw new EntityNotFoundException("ProjectAssignment", id);
        }
        
        // Remove related project skills
        _dbContext.ProjectSkills.RemoveRange(projectAssignment.ProjectSkills);
        
        // Remove the project assignment
        _dbContext.ProjectAssignments.Remove(projectAssignment);
        
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Project assignment deleted successfully: {ProjectAssignmentId}", id);
    }
    
    /// <summary>
    /// Helper method to get a project assignment with all its details
    /// </summary>
    private async Task<ProjectAssignmentResponse> GetResponseWithDetailsAsync(Guid id)
    {
        var projectAssignment = await _dbContext.ProjectAssignments
            .Include(p => p.Company)
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (projectAssignment == null)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", id);
            throw new EntityNotFoundException("ProjectAssignment", id);
        }
        
        return MapToResponse(projectAssignment);
    }
    
    /// <summary>
    /// Helper method to map a project assignment entity to a response model
    /// </summary>
    private ProjectAssignmentResponse MapToResponse(Models.Entities.ProjectAssignment projectAssignment)
    {
        var response = _mapper.Map<ProjectAssignmentResponse>(projectAssignment);
        
        // Set company name
        if (projectAssignment.Company != null)
        {
            response.CompanyName = projectAssignment.Company.Name;
        }
        
        // Map skills
        if (projectAssignment.ProjectSkills != null)
        {
            response.Skills = projectAssignment.ProjectSkills
                .Where(ps => ps.Skill != null)
                .Select(ps => _mapper.Map<SkillResponse>(ps.Skill!))
                .ToList();
        }
        
        return response;
    }
}
