using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Data;
using SkillBridge.Tests.Common;
using SkillBridge.Tests.Fixtures;

namespace SkillBridge.Tests.Services;

/// <summary>
/// Base class for service tests
/// </summary>
public abstract class ServiceTestBase : DatabaseTestBase
{
    protected readonly IMapper Mapper;

    protected ServiceTestBase()
    {
        // Create a MapperFixture and get the configured mapper
        var mapperFixture = new MapperFixture();
        Mapper = mapperFixture.Mapper;
    }
      /// <summary>
    /// Seed the database with test data
    /// </summary>
    protected async Task SeedDatabaseAsync()
    {
        await SeedSkillsAsync();
        await SeedCompaniesAsync();
        
        // Get the first company to use for project assignments
        var company = await DbContext.Companies.FirstAsync();
        await SeedProjectAssignmentsAsync(company.Id);
    }
    
    /// <summary>
    /// Seed the database with test skills
    /// </summary>
    protected async Task SeedSkillsAsync()
    {
        var skills = TestDataGenerator.CreateTestSkills(5);
        await DbContext.Skills.AddRangeAsync(skills);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Seed the database with test companies
    /// </summary>
    protected async Task SeedCompaniesAsync(int count = 5)
    {
        var companies = TestDataGenerator.CreateTestCompanies(count);
        await DbContext.Companies.AddRangeAsync(companies);
        await DbContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Seed the database with test project assignments for a company
    /// </summary>
    protected async Task SeedProjectAssignmentsAsync(Guid? companyId = null, int count = 5)
    {
        var assignments = TestDataGenerator.CreateTestProjectAssignments(count, companyId);
        await DbContext.ProjectAssignments.AddRangeAsync(assignments);
        await DbContext.SaveChangesAsync();
    }
}
