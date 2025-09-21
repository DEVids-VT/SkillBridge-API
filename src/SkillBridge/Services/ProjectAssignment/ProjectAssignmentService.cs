using Auth0.ManagementApi.Paging;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Models.Specifications;
using SkillBridge.Services.Skill;
using System.Threading;

namespace SkillBridge.Services.ProjectAssignment;

/// <summary>
/// Implementation of the project assignment service
/// </summary>
public class ProjectAssignmentService : IProjectAssignmentService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectAssignmentService> _logger;
    private readonly ISkillService _skillService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectAssignmentService"/> class.
    /// </summary>
    public ProjectAssignmentService(
        AppDbContext dbContext, 
        IMapper mapper, 
        ILogger<ProjectAssignmentService> logger,
        ISkillService skillService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
        _skillService = skillService;
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
        
        // Validate skills using SkillService
        List<Guid> validatedSkillIds = new();
        if (request.SkillIds.Any())
        {
            validatedSkillIds = await _skillService.ValidateSkillsExistAsync(request.SkillIds);
        }
        
        // Create the project assignment with skills
        var projectAssignment = _mapper.Map<Models.Entities.ProjectAssignment>(request);
        projectAssignment.CompanyId = companyId;
        
        // Prepare project skills if any skills are specified
        if (validatedSkillIds.Any())
        {
            projectAssignment.ProjectSkills = validatedSkillIds.Select(skillId => new ProjectSkill
            {
                SkillId = skillId
            }).ToList();
        }
        
        // Create assignment tasks if any are specified
        if (request.Tasks != null && request.Tasks.Any())
        {
            _logger.LogInformation("Creating {TaskCount} tasks for project assignment", request.Tasks.Count);
            
            projectAssignment.Tasks = request.Tasks.Select(taskRequest => 
            {
                var task = _mapper.Map<AssignmentTask>(taskRequest);
                // No need to set ProjectAssignmentId as EF Core will handle this
                return task;
            }).ToList();
        }
        
        // Save everything in a single transaction
        await _dbContext.ProjectAssignments.AddAsync(projectAssignment);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Project assignment created successfully with ID: {ProjectAssignmentId}", projectAssignment.Id);
        
        // Return the full project assignment with skills and tasks
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
            .Include(p => p.Tasks.OrderBy(t => t.Sequence))
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (projectAssignment == null)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", id);
            throw new EntityNotFoundException("ProjectAssignment", id);
        }        
        return _mapper.Map<ProjectAssignmentResponse>(projectAssignment);
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
            .Include(p => p.Tasks.OrderBy(t => t.Sequence))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
          _logger.LogInformation("Retrieved {ProjectAssignmentCount} project assignments", projectAssignments.Count);
        
        return projectAssignments.Select(p => _mapper.Map<ProjectAssignmentResponse>(p));
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
            .Include(p => p.Tasks.OrderBy(t => t.Sequence))
            .Where(p => p.CompanyId == companyId)
            .ToListAsync();
          _logger.LogInformation("Retrieved {ProjectAssignmentCount} project assignments for company ID: {CompanyId}", 
            projectAssignments.Count, companyId);
        
        return projectAssignments.Select(p => _mapper.Map<ProjectAssignmentResponse>(p));
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
        
        // Validate skills using SkillService
        List<Guid> validatedSkillIds = new();
        if (request.SkillIds.Any())
        {
            validatedSkillIds = await _skillService.ValidateSkillsExistAsync(request.SkillIds);
        }
        
        // Update skills - remove existing and add new ones
        _dbContext.ProjectSkills.RemoveRange(projectAssignment.ProjectSkills);
        
        if (validatedSkillIds.Any())
        {
            var projectSkills = validatedSkillIds.Select(skillId => new ProjectSkill
            {
                ProjectAssignmentId = projectAssignment.Id,
                SkillId = skillId
            }).ToList();
            
            await _dbContext.ProjectSkills.AddRangeAsync(projectSkills);
        }
        
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
    /// Creates a new task for a project assignment
    /// </summary>
    public async Task<AssignmentTaskResponse> CreateTaskAsync(Guid projectId, CreateAssignmentTaskRequest request)
    {
        _logger.LogInformation("Creating new task for project assignment ID: {ProjectAssignmentId}", projectId);
        
        // Verify project assignment exists
        var projectExists = await _dbContext.ProjectAssignments.AnyAsync(p => p.Id == projectId);
        if (!projectExists)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", projectId);
            throw new EntityNotFoundException("ProjectAssignment", projectId);
        }
        
        // Create the task
        var task = _mapper.Map<AssignmentTask>(request);
        task.ProjectAssignmentId = projectId;
        
        // Save the task
        await _dbContext.AssignmentTasks.AddAsync(task);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Task created successfully with ID: {TaskId} for project assignment ID: {ProjectAssignmentId}", 
            task.Id, projectId);
        
        return _mapper.Map<AssignmentTaskResponse>(task);
    }
    
    /// <summary>
    /// Gets all tasks for a project assignment
    /// </summary>
    public async Task<IEnumerable<AssignmentTaskResponse>> GetTasksAsync(Guid projectId)
    {
        _logger.LogInformation("Retrieving tasks for project assignment ID: {ProjectAssignmentId}", projectId);
        
        // Verify project assignment exists
        var projectExists = await _dbContext.ProjectAssignments.AnyAsync(p => p.Id == projectId);
        if (!projectExists)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", projectId);
            throw new EntityNotFoundException("ProjectAssignment", projectId);
        }
        
        var tasks = await _dbContext.AssignmentTasks
            .Where(t => t.ProjectAssignmentId == projectId)
            .OrderBy(t => t.Sequence)
            .ToListAsync();
        
        _logger.LogInformation("Retrieved {TaskCount} tasks for project assignment ID: {ProjectAssignmentId}",
            tasks.Count, projectId);
        
        return tasks.Select(t => _mapper.Map<AssignmentTaskResponse>(t));
    }
    
    /// <summary>
    /// Gets a specific task from a project assignment
    /// </summary>
    public async Task<AssignmentTaskResponse> GetTaskByIdAsync(Guid projectId, Guid taskId)
    {
        _logger.LogInformation("Retrieving task with ID: {TaskId} for project assignment ID: {ProjectAssignmentId}", 
            taskId, projectId);
        
        var task = await _dbContext.AssignmentTasks
            .FirstOrDefaultAsync(t => t.ProjectAssignmentId == projectId && t.Id == taskId);
        
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found in project assignment ID: {ProjectAssignmentId}", 
                taskId, projectId);
            throw new EntityNotFoundException("AssignmentTask", taskId);
        }
        
        return _mapper.Map<AssignmentTaskResponse>(task);
    }
    
    /// <summary>
    /// Updates a specific task in a project assignment
    /// </summary>
    public async Task<AssignmentTaskResponse> UpdateTaskAsync(Guid projectId, Guid taskId, UpdateAssignmentTaskRequest request)
    {
        _logger.LogInformation("Updating task with ID: {TaskId} for project assignment ID: {ProjectAssignmentId}",
            taskId, projectId);
        
        var task = await _dbContext.AssignmentTasks
            .FirstOrDefaultAsync(t => t.ProjectAssignmentId == projectId && t.Id == taskId);
        
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found in project assignment ID: {ProjectAssignmentId}",
                taskId, projectId);
            throw new EntityNotFoundException("AssignmentTask", taskId);
        }
        
        // Update the task properties
        _mapper.Map(request, task);
        task.UpdatedAt = DateTime.UtcNow;
        
        // Save changes
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Task updated successfully with ID: {TaskId} for project assignment ID: {ProjectAssignmentId}",
            taskId, projectId);
        
        return _mapper.Map<AssignmentTaskResponse>(task);
    }
    
    /// <summary>
    /// Deletes a specific task from a project assignment
    /// </summary>
    public async Task DeleteTaskAsync(Guid projectId, Guid taskId)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId} from project assignment ID: {ProjectAssignmentId}",
            taskId, projectId);
        
        var task = await _dbContext.AssignmentTasks
            .FirstOrDefaultAsync(t => t.ProjectAssignmentId == projectId && t.Id == taskId);
        
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found in project assignment ID: {ProjectAssignmentId}",
                taskId, projectId);
            throw new EntityNotFoundException("AssignmentTask", taskId);
        }
        
        // Remove the task
        _dbContext.AssignmentTasks.Remove(task);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Task deleted successfully with ID: {TaskId} from project assignment ID: {ProjectAssignmentId}",
            taskId, projectId);
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
            .Include(p => p.Tasks.OrderBy(t => t.Sequence))
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (projectAssignment == null)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", id);
            throw new EntityNotFoundException("ProjectAssignment", id);
        }
        
        return _mapper.Map<ProjectAssignmentResponse>(projectAssignment);
    }

    /// <summary>
    /// Searches project assignments based on a specification with pagination
    /// </summary>

    // TODO: Implement pagination for search results AND in the Intreface IProjectAssignmentService
    // public async Task<PagedList<ProjectAssignmentResponse>>
    public async Task<IEnumerable<ProjectAssignmentResponse>> SearchAsync(Specification<Models.Entities.ProjectAssignment> specification,
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ProjectAssignments
            .Include(p => p.Company)
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .Where(specification);

        /* var totalCount = await query.CountAsync(cancellationToken);

         var data = await query
             .Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync(cancellationToken);

         var mapped = _mapper.Map<List<ProjectAssignmentResponse>>(data);

         var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

         return new PagedList<ProjectAssignmentResponse>(mapped, pageNumber, pageSize, totalPages, totalCount);*/

        // Delete this when pagination is implemented
        var results = await query.ToListAsync();
        return results.Select(p => _mapper.Map<ProjectAssignmentResponse>(p));
}

    /// Completes/Uncompletes a specific task in a project assignment
    /// </summary>
    public async Task<AssignmentTaskResponse> CompleteTaskAsync(Guid projectId, Guid taskId)
    {
        var projectAssignment = await _dbContext.ProjectAssignments
           .Include(p => p.Company)
           .Include(p => p.ProjectSkills)
               .ThenInclude(ps => ps.Skill)
           .Include(p => p.Tasks.OrderBy(t => t.Sequence))
           .FirstOrDefaultAsync(p => p.Id == projectId);

        if (projectAssignment == null)
        {
            _logger.LogWarning("Project assignment with ID {ProjectAssignmentId} not found", projectId);
            throw new EntityNotFoundException("ProjectAssignment", projectId);
        }

        var task = projectAssignment.Tasks.FirstOrDefault(t => t.Id == taskId);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found in project assignment ID: {ProjectAssignmentId}",
                taskId, projectId);
            throw new EntityNotFoundException("AssignmentTask", taskId);
        }

        if (task.IsCompleted == false)
        {
            task.IsCompleted = true;
        }
        else if (task.IsCompleted == true)
        {
            task.IsCompleted = false;
        }
        task.UpdatedAt = DateTime.UtcNow;

        _dbContext.AssignmentTasks.Update(task);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Task updated successfully with ID: {TaskId} for project assignment ID: {ProjectAssignmentId}",
           taskId, projectId);

        return _mapper.Map<AssignmentTaskResponse>(task);
    }
}
