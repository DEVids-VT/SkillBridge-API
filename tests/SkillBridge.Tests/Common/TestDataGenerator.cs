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
    public static Company CreateTestCompany(
        Guid? id = null, 
        string? name = null, 
        string? about = null,
        string? logoUrl = null,
        string? bannerUrl = null,
        string? activities = null,
        string? sector = null,
        string? headOfficeLocation = null,
        string? technologies = null,
        int? yearEstablished = null,
        bool? hasOfficesInBulgaria = null,
        string? bulgarianOfficeLocations = null,
        int? employeesInBulgaria = null,
        int? employeesWorldwide = null,
        string? whyWorkWithUs = null,
        string? websiteUrl = null,
        string? contactInfo = null,
        string? auth0UserId = null)
    {
        return new Company
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? "Test Company",
            About = about ?? "Test About Text",
            LogoUrl = logoUrl ?? "https://example.com/logo.png",
            BannerUrl = bannerUrl ?? "https://example.com/banner.png",
            Activities = activities ?? "Product company",
            Sector = sector ?? "IoT",
            HeadOfficeLocation = headOfficeLocation ?? "Sofia, Bulgaria",
            Technologies = technologies ?? "C#, .NET, React",
            YearEstablished = yearEstablished ?? 2020,
            HasOfficesInBulgaria = hasOfficesInBulgaria ?? true,
            BulgarianOfficeLocations = bulgarianOfficeLocations ?? "Sofia",
            EmployeesInBulgaria = employeesInBulgaria ?? 50,
            EmployeesWorldwide = employeesWorldwide ?? 200,
            WhyWorkWithUs = whyWorkWithUs ?? "Great culture and benefits",
            WebsiteUrl = websiteUrl ?? "https://example.com",
            ContactInfo = contactInfo ?? "contact@example.com",
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
                About = $"About text for company {i}",
                LogoUrl = $"https://example.com/company{i}/logo.png",
                BannerUrl = $"https://example.com/company{i}/banner.png",
                Activities = "Product company",
                Sector = "IoT",
                HeadOfficeLocation = "Sofia, Bulgaria",
                Technologies = "C#, .NET, React",
                YearEstablished = 2020,
                HasOfficesInBulgaria = true,
                BulgarianOfficeLocations = "Sofia",
                EmployeesInBulgaria = 50,
                EmployeesWorldwide = 200,
                WhyWorkWithUs = "Great culture and benefits",
                WebsiteUrl = $"https://company{i}.example.com",
                ContactInfo = $"contact@company{i}.example.com",
                Auth0UserId = $"auth0|{i}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            });
        }
        
        return companies;
    }    /// <summary>
    /// Creates a test create company request
    /// </summary>
    public static CreateCompanyRequest CreateTestCreateCompanyRequest(
        string? name = null, 
        string? about = null,
        string? logoUrl = null,
        string? bannerUrl = null,
        string? activities = null,
        string? sector = null,
        string? headOfficeLocation = null,
        string? technologies = null,
        int? yearEstablished = null,
        bool? hasOfficesInBulgaria = null,
        string? bulgarianOfficeLocations = null,
        int? employeesInBulgaria = null,
        int? employeesWorldwide = null,
        string? whyWorkWithUs = null,
        string? websiteUrl = null,
        string? contactInfo = null)
    {
        return new CreateCompanyRequest
        {
            Name = name ?? "Test Company",
            About = about ?? "Test About Text",
            LogoUrl = logoUrl ?? "https://example.com/logo.png",
            BannerUrl = bannerUrl ?? "https://example.com/banner.png",
            Activities = activities ?? "Product company",
            Sector = sector ?? "IoT",
            HeadOfficeLocation = headOfficeLocation ?? "Sofia, Bulgaria",
            Technologies = technologies ?? "C#, .NET, React",
            YearEstablished = yearEstablished ?? 2020,
            HasOfficesInBulgaria = hasOfficesInBulgaria ?? true,
            BulgarianOfficeLocations = bulgarianOfficeLocations ?? "Sofia",
            EmployeesInBulgaria = employeesInBulgaria ?? 50,
            EmployeesWorldwide = employeesWorldwide ?? 200,
            WhyWorkWithUs = whyWorkWithUs ?? "Great culture and benefits",
            WebsiteUrl = websiteUrl ?? "https://example.com",
            ContactInfo = contactInfo ?? "contact@example.com"
        };
    }
    
    /// <summary>
    /// Creates a test update company request
    /// </summary>
    public static UpdateCompanyRequest CreateTestUpdateCompanyRequest(
        string? name = null,
        string? about = null,
        string? logoUrl = null,
        string? bannerUrl = null,
        string? activities = null,
        string? sector = null,
        string? headOfficeLocation = null,
        string? technologies = null,
        int? yearEstablished = null,
        bool? hasOfficesInBulgaria = null,
        string? bulgarianOfficeLocations = null,
        int? employeesInBulgaria = null,
        int? employeesWorldwide = null,
        string? whyWorkWithUs = null,
        string? websiteUrl = null,
        string? contactInfo = null)
    {
        return new UpdateCompanyRequest
        {
            Name = name ?? "Updated Company",
            About = about ?? "Updated About Text",
            LogoUrl = logoUrl ?? "https://example.com/updated-logo.png",
            BannerUrl = bannerUrl ?? "https://example.com/updated-banner.png",
            Activities = activities ?? "Updated Product company",
            Sector = sector ?? "Updated IoT",
            HeadOfficeLocation = headOfficeLocation ?? "Updated Sofia, Bulgaria",
            Technologies = technologies ?? "Updated C#, .NET, React",
            YearEstablished = yearEstablished ?? 2020,
            HasOfficesInBulgaria = hasOfficesInBulgaria ?? true,
            BulgarianOfficeLocations = bulgarianOfficeLocations ?? "Updated Sofia",
            EmployeesInBulgaria = employeesInBulgaria ?? 60,
            EmployeesWorldwide = employeesWorldwide ?? 250,
            WhyWorkWithUs = whyWorkWithUs ?? "Updated culture and benefits",
            WebsiteUrl = websiteUrl ?? "https://updated-example.com",
            ContactInfo = contactInfo ?? "updated-contact@example.com"
        };
    }
    
    /// <summary>
    /// Creates a test company response
    /// </summary>
    public static CompanyResponse CreateTestCompanyResponse(
        Guid? id = null,
        string? name = null,
        string? about = null,
        string? logoUrl = null,
        string? bannerUrl = null,
        string? activities = null,
        string? sector = null,
        string? headOfficeLocation = null,
        string? technologies = null,
        int? yearEstablished = null,
        bool? hasOfficesInBulgaria = null,
        string? bulgarianOfficeLocations = null,
        int? employeesInBulgaria = null,
        int? employeesWorldwide = null,
        string? whyWorkWithUs = null,
        string? websiteUrl = null,
        string? contactInfo = null,
        string? auth0UserId = null)
    {
        return new CompanyResponse
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? "Test Company",
            About = about ?? "Test About Text",
            LogoUrl = logoUrl ?? "https://example.com/logo.png",
            BannerUrl = bannerUrl ?? "https://example.com/banner.png",
            Activities = activities ?? "Product company",
            Sector = sector ?? "IoT",
            HeadOfficeLocation = headOfficeLocation ?? "Sofia, Bulgaria",
            Technologies = technologies ?? "C#, .NET, React",
            YearEstablished = yearEstablished ?? 2020,
            HasOfficesInBulgaria = hasOfficesInBulgaria ?? true,
            BulgarianOfficeLocations = bulgarianOfficeLocations ?? "Sofia",
            EmployeesInBulgaria = employeesInBulgaria ?? 50,
            EmployeesWorldwide = employeesWorldwide ?? 200,
            WhyWorkWithUs = whyWorkWithUs ?? "Great culture and benefits",
            WebsiteUrl = websiteUrl ?? "https://example.com",
            ContactInfo = contactInfo ?? "contact@example.com",
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
