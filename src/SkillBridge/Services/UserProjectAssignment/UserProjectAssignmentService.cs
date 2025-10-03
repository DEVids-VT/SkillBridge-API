using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.UserProjectAssignment;

/// <summary>
/// Implementation of the user project assignment service
/// </summary>
public class UserProjectAssignmentService : IUserProjectAssignmentService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<UserProjectAssignmentService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProjectAssignmentService"/> class.
    /// </summary>
    public UserProjectAssignmentService(
        AppDbContext dbContext,
        IMapper mapper,
        ILogger<UserProjectAssignmentService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Claims a project assignment for a user
    /// </summary>
    public async Task<UserProjectAssignmentResponse> ClaimProjectAsync(string userId, ClaimProjectRequest request)
    {
        _logger.LogInformation("User {UserId} claiming project assignment {ProjectAssignmentId}", 
            userId, request.ProjectAssignmentId);
        
        // Verify user profile exists
        var userExists = await _dbContext.UserProfiles.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            _logger.LogWarning("User profile with ID {UserId} not found", userId);
            throw new EntityNotFoundException("UserProfile", userId);
        }
        
        // Verify project assignment exists and is published
        var projectAssignment = await _dbContext.ProjectAssignments
            .Include(p => p.Company)
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == request.ProjectAssignmentId);
            
        if (projectAssignment == null)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", request.ProjectAssignmentId);
            throw new EntityNotFoundException("ProjectAssignment", request.ProjectAssignmentId);
        }
        
        //if (projectAssignment.Status != Models.Enums.ProjectAssignmentStatus.Published)
        //{
        //    _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} is not published", 
        //        request.ProjectAssignmentId);
        //    throw new InvalidOperationException("Cannot claim a project assignment that is not published");
        //}
        
        // Check if the user has already claimed this project
        var existingClaim = await _dbContext.UserProjectAssignments
            .FirstOrDefaultAsync(up => up.UserProfileId == userId && up.ProjectAssignmentId == request.ProjectAssignmentId);
            
        if (existingClaim != null)
        {
            _logger.LogWarning("User {UserId} has already claimed project assignment {ProjectAssignmentId}",
                userId, request.ProjectAssignmentId);
            throw new InvalidOperationException("You have already claimed this project assignment");
        }
        
        // Create new user project assignment
        var userProjectAssignment = new Models.Entities.UserProjectAssignment
        {
            UserProfileId = userId,
            ProjectAssignmentId = request.ProjectAssignmentId,
            ClaimedAt = DateTime.UtcNow,
            IsCompleted = false,
            Deadline = DateTime.UtcNow + projectAssignment.Duration,
        };
        
        // Save to database
        await _dbContext.UserProjectAssignments.AddAsync(userProjectAssignment);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} successfully claimed project assignment {ProjectAssignmentId}",
            userId, request.ProjectAssignmentId);
        
        // Build the response
        var response = new UserProjectAssignmentResponse
        {
            ProjectAssignment = _mapper.Map<ProjectAssignmentResponse>(projectAssignment),
            ClaimedAt = userProjectAssignment.ClaimedAt,
            IsCompleted = userProjectAssignment.IsCompleted,
            CompletedAt = userProjectAssignment.CompletedAt,
            Deadline = userProjectAssignment.Deadline
        };
        
        return response;
    }

    /// <summary>
    /// Gets all project assignments claimed by a user
    /// </summary>
    public async Task<IEnumerable<UserProjectAssignmentResponse>> GetUserProjectsAsync(string userId)
    {
        _logger.LogInformation("Getting all project assignments claimed by user {UserId}", userId);
        
        // Verify user profile exists
        var userExists = await _dbContext.UserProfiles.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            _logger.LogWarning("User profile with ID {UserId} not found", userId);
            throw new EntityNotFoundException("UserProfile", userId);
        }
        
        // Get all user project assignments with related data
        var userProjectAssignments = await _dbContext.UserProjectAssignments
            .Where(upa => upa.UserProfileId == userId)
            .Include(upa => upa.ProjectAssignment)
                .ThenInclude(pa => pa.Company)
            .Include(upa => upa.ProjectAssignment)
                .ThenInclude(pa => pa.ProjectSkills)
                    .ThenInclude(ps => ps.Skill)
            .Include(upa => upa.ProjectAssignment)
                .ThenInclude(pa => pa.Tasks)
            .OrderByDescending(upa => upa.ClaimedAt)
            .ToListAsync();
            
        _logger.LogInformation("Found {Count} project assignments claimed by user {UserId}",
            userProjectAssignments.Count, userId);
        
        // Map to response model
        var response = userProjectAssignments.Select(upa => new UserProjectAssignmentResponse
        {
            ProjectAssignment = _mapper.Map<ProjectAssignmentResponse>(upa.ProjectAssignment),
            ClaimedAt = upa.ClaimedAt,
            IsCompleted = upa.IsCompleted,
            CompletedAt = upa.CompletedAt
        });
        
        return response;
    }

    /// <summary>
    /// Marks a project assignment as completed by a user
    /// </summary>
    public async Task<UserProjectAssignmentResponse> CompleteProjectAsync(string userId, Guid projectAssignmentId)
    {
        _logger.LogInformation("User {UserId} marking project assignment {ProjectAssignmentId} as completed",
            userId, projectAssignmentId);
        
        // Find the user project assignment
        var userProjectAssignment = await _dbContext.UserProjectAssignments
            .Include(upa => upa.ProjectAssignment)
                .ThenInclude(pa => pa.Company)
            .Include(upa => upa.ProjectAssignment)
                .ThenInclude(pa => pa.ProjectSkills)
                    .ThenInclude(ps => ps.Skill)
            .Include(upa => upa.ProjectAssignment)
                .ThenInclude(pa => pa.Tasks)
            .FirstOrDefaultAsync(upa => 
                upa.UserProfileId == userId && 
                upa.ProjectAssignmentId == projectAssignmentId);
                
        if (userProjectAssignment == null)
        {
            _logger.LogWarning(
                "User project assignment not found for user {UserId} and project {ProjectAssignmentId}",
                userId, projectAssignmentId);
            throw new EntityNotFoundException(
                "UserProjectAssignment", 
                $"User: {userId}, Project: {projectAssignmentId}");
        }
        
        if (userProjectAssignment.IsCompleted)
        {
            _logger.LogWarning(
                "Project assignment {ProjectAssignmentId} already marked as completed by user {UserId}",
                projectAssignmentId, userId);
            throw new InvalidOperationException("This project assignment is already marked as completed");
        }
        
        // Mark as completed
        userProjectAssignment.IsCompleted = true;
        userProjectAssignment.CompletedAt = DateTime.UtcNow;
        
        // Save changes
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation(
            "User {UserId} successfully marked project assignment {ProjectAssignmentId} as completed",
            userId, projectAssignmentId);
        
        // Build response
        var response = new UserProjectAssignmentResponse
        {
            ProjectAssignment = _mapper.Map<ProjectAssignmentResponse>(userProjectAssignment.ProjectAssignment),
            ClaimedAt = userProjectAssignment.ClaimedAt,
            IsCompleted = userProjectAssignment.IsCompleted,
            CompletedAt = userProjectAssignment.CompletedAt
        };
        
        return response;
    }
}