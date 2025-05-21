using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Tests.Common;

/// <summary>
/// Provides test data for test cases
/// </summary>
public static class TestDataGenerator
{
    /// <summary>
    /// Creates a test skill entity
    /// </summary>
    public static Skill CreateTestSkill(Guid? id = null, string? name = null, string? description = null)
    {
        return new Skill
        {
            Id = id ?? TestConstants.Skill.ValidId,
            Name = name ?? TestConstants.Skill.ValidName,
            Description = description ?? TestConstants.Skill.ValidDescription
        };
    }
    
    /// <summary>
    /// Creates a list of test skills
    /// </summary>
    public static List<Skill> CreateTestSkills(int count)
    {
        var skills = new List<Skill>();
        
        for (var i = 0; i < count; i++)
        {
            skills.Add(new Skill
            {
                Id = Guid.NewGuid(),
                Name = $"Skill {i}",
                Description = $"Description {i}"
            });
        }
        
        return skills;
    }
    
    /// <summary>
    /// Creates a test create skill request
    /// </summary>
    public static CreateSkillRequest CreateTestCreateSkillRequest(string? name = null, string? description = null)
    {
        return new CreateSkillRequest
        {
            Name = name ?? TestConstants.Skill.ValidName,
            Description = description ?? TestConstants.Skill.ValidDescription
        };
    }
    
    /// <summary>
    /// Creates a test update skill request
    /// </summary>
    public static UpdateSkillRequest CreateTestUpdateSkillRequest(string? name = null, string? description = null)
    {
        return new UpdateSkillRequest
        {
            Name = name ?? TestConstants.Skill.ValidName,
            Description = description ?? TestConstants.Skill.ValidDescription
        };
    }
    
    /// <summary>
    /// Creates a test skill response
    /// </summary>
    public static SkillResponse CreateTestSkillResponse(Guid? id = null, string? name = null, string? description = null)
    {
        return new SkillResponse
        {
            Id = id ?? TestConstants.Skill.ValidId,
            Name = name ?? TestConstants.Skill.ValidName,
            Description = description ?? TestConstants.Skill.ValidDescription
        };
    }
    
    /// <summary>
    /// Creates a test company entity
    /// </summary>
    public static Company CreateTestCompany(Guid? id = null, string? name = null, string? description = null, string? auth0UserId = null)
    {
        return new Company
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? "Test Company",
            Description = description ?? "Test Description",
            Auth0UserId = auth0UserId ?? "auth0|12345678",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }
    
    /// <summary>
    /// Creates a list of test companies
    /// </summary>
    public static List<Company> CreateTestCompanies(int count)
    {
        var companies = new List<Company>();
        
        for (var i = 0; i < count; i++)
        {
            companies.Add(new Company
            {
                Id = Guid.NewGuid(),
                Name = $"Company {i}",
                Description = $"Description {i}",
                Auth0UserId = $"auth0|{i}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            });
        }
        
        return companies;
    }
    
    /// <summary>
    /// Creates a test create company request
    /// </summary>
    public static CreateCompanyRequest CreateTestCreateCompanyRequest(string? name = null, string? description = null, string? auth0UserId = null)
    {
        return new CreateCompanyRequest
        {
            Name = name ?? "Test Company",
            Description = description ?? "Test Description",
            Auth0UserId = auth0UserId ?? "auth0|12345678"
        };
    }
    
    /// <summary>
    /// Creates a test update company request
    /// </summary>
    public static UpdateCompanyRequest CreateTestUpdateCompanyRequest(string? name = null, string? description = null)
    {
        return new UpdateCompanyRequest
        {
            Name = name ?? "Updated Company",
            Description = description ?? "Updated Description"
        };
    }
    
    /// <summary>
    /// Creates a test company response
    /// </summary>
    public static CompanyResponse CreateTestCompanyResponse(Guid? id = null, string? name = null, string? description = null, string? auth0UserId = null)
    {
        return new CompanyResponse
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? "Test Company",
            Description = description ?? "Test Description",
            Auth0UserId = auth0UserId ?? "auth0|12345678",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }
    
    /// <summary>
    /// Creates a test project assignment entity
    /// </summary>
    public static ProjectAssignment CreateTestProjectAssignment(
        Guid? id = null, 
        string? title = null, 
        string? description = null, 
        DateTime? deadline = null,
        ProjectAssignmentStatus? status = null,
        Guid? companyId = null)
    {
        return new ProjectAssignment
        {
            Id = id ?? Guid.NewGuid(),
            Title = title ?? "Test Project",
            Description = description ?? "Test Project Description",
            Deadline = deadline ?? DateTime.UtcNow.AddDays(30),
            Status = status ?? ProjectAssignmentStatus.Draft,
            CompanyId = companyId ?? Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            ProjectSkills = new List<ProjectSkill>()
        };
    }
    
    /// <summary>
    /// Creates a list of test project assignments
    /// </summary>
    public static List<ProjectAssignment> CreateTestProjectAssignments(int count, Guid? companyId = null)
    {
        var assignments = new List<ProjectAssignment>();
        var company = companyId ?? Guid.NewGuid();
        
        for (var i = 0; i < count; i++)
        {
            assignments.Add(new ProjectAssignment
            {
                Id = Guid.NewGuid(),
                Title = $"Project {i}",
                Description = $"Description {i}",
                Deadline = DateTime.UtcNow.AddDays(30 + i),
                Status = ProjectAssignmentStatus.Draft,
                CompanyId = company,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                ProjectSkills = new List<ProjectSkill>()
            });
        }
        
        return assignments;
    }
    
    /// <summary>
    /// Creates a test create project assignment request
    /// </summary>
    public static CreateProjectAssignmentRequest CreateTestCreateProjectAssignmentRequest(
        string? title = null, 
        string? description = null, 
        DateTime? deadline = null,
        ProjectAssignmentStatus? status = null,
        List<Guid>? skillIds = null)
    {
        return new CreateProjectAssignmentRequest
        {
            Title = title ?? "Test Project",
            Description = description ?? "Test Project Description",
            Deadline = deadline ?? DateTime.UtcNow.AddDays(30),
            Status = status ?? ProjectAssignmentStatus.Draft,
            SkillIds = skillIds ?? new List<Guid>()
        };
    }
    
    /// <summary>
    /// Creates a test update project assignment request
    /// </summary>
    public static UpdateProjectAssignmentRequest CreateTestUpdateProjectAssignmentRequest(
        string? title = null, 
        string? description = null, 
        DateTime? deadline = null,
        ProjectAssignmentStatus? status = null,
        List<Guid>? skillIds = null)
    {
        return new UpdateProjectAssignmentRequest
        {
            Title = title ?? "Updated Project",
            Description = description ?? "Updated Project Description",
            Deadline = deadline ?? DateTime.UtcNow.AddDays(30),
            Status = status ?? ProjectAssignmentStatus.Published,
            SkillIds = skillIds ?? new List<Guid>()
        };
    }
    
    /// <summary>
    /// Creates a test project assignment response
    /// </summary>
    public static ProjectAssignmentResponse CreateTestProjectAssignmentResponse(
        Guid? id = null, 
        string? title = null, 
        string? description = null, 
        DateTime? deadline = null,
        ProjectAssignmentStatus? status = null,
        Guid? companyId = null,
        string? companyName = null,
        List<SkillResponse>? skills = null)
    {
        return new ProjectAssignmentResponse
        {
            Id = id ?? Guid.NewGuid(),
            Title = title ?? "Test Project",
            Description = description ?? "Test Project Description",
            Deadline = deadline ?? DateTime.UtcNow.AddDays(30),
            Status = status ?? ProjectAssignmentStatus.Draft,
            CompanyId = companyId ?? Guid.NewGuid(),
            CompanyName = companyName ?? "Test Company",
            Skills = skills ?? new List<SkillResponse>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }
}
