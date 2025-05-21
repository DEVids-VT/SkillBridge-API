using Microsoft.Extensions.Logging;
using Moq;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services;
using SkillBridge.Tests.Common;

namespace SkillBridge.Tests.Services;

/// <summary>
/// Tests for the <see cref="ProjectAssignmentService"/> class
/// </summary>
public class ProjectAssignmentServiceTests : ServiceTestBase
{
    private readonly ProjectAssignmentService _projectAssignmentService;
    private readonly Mock<ILogger<ProjectAssignmentService>> _loggerMock;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectAssignmentServiceTests"/> class
    /// </summary>
    public ProjectAssignmentServiceTests()
    {
        _loggerMock = MockLoggerFactory.CreateLogger<ProjectAssignmentService>();
        _projectAssignmentService = new ProjectAssignmentService(DbContext, Mapper, _loggerMock.Object);
    }
    
    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsProjectAssignmentResponse()
    {
        // Arrange
        var company = TestDataGenerator.CreateTestCompany();
        await DbContext.Companies.AddAsync(company);

        // Add skills to use in request
        var skills = TestDataGenerator.CreateTestSkills(3);
        await DbContext.Skills.AddRangeAsync(skills);
        await DbContext.SaveChangesAsync();

        var skillIds = skills.Select(s => s.Id).ToList();
        
        var request = TestDataGenerator.CreateTestCreateProjectAssignmentRequest(
            skillIds: skillIds
        );
        
        // Act
        var result = await _projectAssignmentService.CreateAsync(company.Id, request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Status, result.Status);
        Assert.Equal(company.Id, result.CompanyId);
        Assert.NotEqual(Guid.Empty, result.Id);
        
        // Verify skills were added
        Assert.Equal(3, result.Skills.Count);
        foreach (var skill in skills)
        {
            Assert.Contains(result.Skills, s => s.Id == skill.Id);
        }
        
        // Verify it was added to database
        var dbProjectAssignment = await DbContext.ProjectAssignments.FindAsync(result.Id);
        Assert.NotNull(dbProjectAssignment);
        Assert.Equal(request.Title, dbProjectAssignment.Title);
        Assert.Equal(request.Description, dbProjectAssignment.Description);
        Assert.Equal(request.Status, dbProjectAssignment.Status);
        Assert.Equal(company.Id, dbProjectAssignment.CompanyId);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Creating new project assignment for company ID: {company.Id}", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            "Project assignment created successfully with ID:", Times.Once());
    }
    
    [Fact]
    public async Task CreateAsync_NonExistingCompany_ThrowsArgumentException()
    {
        // Arrange
        var nonExistingCompanyId = Guid.NewGuid();
        var request = TestDataGenerator.CreateTestCreateProjectAssignmentRequest();
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _projectAssignmentService.CreateAsync(nonExistingCompanyId, request));
        
        Assert.Contains($"Company with ID {nonExistingCompanyId} not found", exception.Message);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Creating new project assignment for company ID: {nonExistingCompanyId}", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Warning, 
            $"Company with ID {nonExistingCompanyId} not found", Times.Once());
    }
    
    [Fact]
    public async Task CreateAsync_NonExistingSkills_ThrowsArgumentException()
    {
        // Arrange
        var company = TestDataGenerator.CreateTestCompany();
        await DbContext.Companies.AddAsync(company);
        await DbContext.SaveChangesAsync();
        
        var nonExistingSkillId = Guid.NewGuid();
        var request = TestDataGenerator.CreateTestCreateProjectAssignmentRequest(
            skillIds: new List<Guid> { nonExistingSkillId }
        );
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _projectAssignmentService.CreateAsync(company.Id, request));
        
        Assert.Contains($"One or more skills not found: {nonExistingSkillId}", exception.Message);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Warning, 
            $"One or more skills not found:", Times.Once());
    }
    
    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProjectAssignmentResponse()
    {
        // Arrange
        var company = TestDataGenerator.CreateTestCompany();
        await DbContext.Companies.AddAsync(company);
        
        var projectAssignment = TestDataGenerator.CreateTestProjectAssignment(companyId: company.Id);
        await DbContext.ProjectAssignments.AddAsync(projectAssignment);
        await DbContext.SaveChangesAsync();
        
        // Act
        var result = await _projectAssignmentService.GetByIdAsync(projectAssignment.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectAssignment.Id, result.Id);
        Assert.Equal(projectAssignment.Title, result.Title);
        Assert.Equal(projectAssignment.Description, result.Description);
        Assert.Equal(projectAssignment.CompanyId, result.CompanyId);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Retrieving project assignment with ID: {projectAssignment.Id}", Times.Once());
    }
    
    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        var result = await _projectAssignmentService.GetByIdAsync(id);
        
        // Assert
        Assert.Null(result);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Retrieving project assignment with ID: {id}", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Warning, 
            $"Project assignment with ID {id} not found", Times.Once());
    }
    
    [Fact]
    public async Task GetAllAsync_ProjectAssignmentsExist_ReturnsAllProjectAssignments()
    {
        // Arrange
        var company = TestDataGenerator.CreateTestCompany();
        await DbContext.Companies.AddAsync(company);
        await DbContext.SaveChangesAsync();
        
        await SeedProjectAssignmentsAsync(company.Id, 5);
        
        // Act
        var result = await _projectAssignmentService.GetAllAsync();
        
        // Assert
        var projectAssignments = result.ToList();
        Assert.NotNull(projectAssignments);
        Assert.Equal(5, projectAssignments.Count);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            "Retrieving all project assignments", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            "Retrieved 5 project assignments", Times.Once());
    }
    
    [Fact]
    public async Task GetByCompanyIdAsync_ExistingCompanyId_ReturnsCompanyProjectAssignments()
    {
        // Arrange
        // Create two companies
        var company1 = TestDataGenerator.CreateTestCompany(name: "Company 1");
        var company2 = TestDataGenerator.CreateTestCompany(name: "Company 2");
        await DbContext.Companies.AddRangeAsync(company1, company2);
        await DbContext.SaveChangesAsync();
        
        // Add 3 project assignments to company 1
        await SeedProjectAssignmentsAsync(company1.Id, 3);
        
        // Add 2 project assignments to company 2
        await SeedProjectAssignmentsAsync(company2.Id, 2);
        
        // Act
        var result = await _projectAssignmentService.GetByCompanyIdAsync(company1.Id);
        
        // Assert
        var projectAssignments = result.ToList();
        Assert.NotNull(projectAssignments);
        Assert.Equal(3, projectAssignments.Count);
        Assert.All(projectAssignments, pa => Assert.Equal(company1.Id, pa.CompanyId));
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Retrieving project assignments for company ID: {company1.Id}", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Retrieved 3 project assignments for company ID: {company1.Id}", Times.Once());
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingId_ReturnsUpdatedProjectAssignment()
    {
        // Arrange
        var company = TestDataGenerator.CreateTestCompany();
        await DbContext.Companies.AddAsync(company);
        
        var projectAssignment = TestDataGenerator.CreateTestProjectAssignment(companyId: company.Id);
        await DbContext.ProjectAssignments.AddAsync(projectAssignment);
        
        // Add skills to use in request
        var skills = TestDataGenerator.CreateTestSkills(2);
        await DbContext.Skills.AddRangeAsync(skills);
        await DbContext.SaveChangesAsync();
        
        var skillIds = skills.Select(s => s.Id).ToList();
        
        var request = TestDataGenerator.CreateTestUpdateProjectAssignmentRequest(
            title: "Updated Project Title",
            description: "Updated Project Description",
            status: ProjectAssignmentStatus.Published,
            skillIds: skillIds
        );
        
        // Act
        var result = await _projectAssignmentService.UpdateAsync(projectAssignment.Id, request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectAssignment.Id, result.Id);
        Assert.Equal("Updated Project Title", result.Title);
        Assert.Equal("Updated Project Description", result.Description);
        Assert.Equal(ProjectAssignmentStatus.Published, result.Status);
        Assert.Equal(company.Id, result.CompanyId);
        
        // Verify skills were updated
        Assert.Equal(2, result.Skills.Count);
        foreach (var skill in skills)
        {
            Assert.Contains(result.Skills, s => s.Id == skill.Id);
        }
        
        // Verify database was updated
        var dbProjectAssignment = await DbContext.ProjectAssignments.FindAsync(projectAssignment.Id);
        Assert.NotNull(dbProjectAssignment);
        Assert.Equal("Updated Project Title", dbProjectAssignment.Title);
        Assert.Equal("Updated Project Description", dbProjectAssignment.Description);
        Assert.Equal(ProjectAssignmentStatus.Published, dbProjectAssignment.Status);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Updating project assignment with ID: {projectAssignment.Id}", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            "Project assignment updated successfully:", Times.Once());
    }
    
    [Fact]
    public async Task UpdateAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = TestDataGenerator.CreateTestUpdateProjectAssignmentRequest();
        
        // Act
        var result = await _projectAssignmentService.UpdateAsync(id, request);
        
        // Assert
        Assert.Null(result);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Updating project assignment with ID: {id}", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Warning, 
            $"Project assignment with ID {id} not found", Times.Once());
    }
    
    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var company = TestDataGenerator.CreateTestCompany();
        await DbContext.Companies.AddAsync(company);
        
        var projectAssignment = TestDataGenerator.CreateTestProjectAssignment(companyId: company.Id);
        
        // Add some skills to the project assignment
        var skills = TestDataGenerator.CreateTestSkills(2);
        await DbContext.Skills.AddRangeAsync(skills);
        await DbContext.SaveChangesAsync();
        
        // Create project skills
        foreach (var skill in skills)
        {
            var projectSkill = new ProjectSkill
            {
                ProjectAssignmentId = projectAssignment.Id,
                SkillId = skill.Id
            };
            projectAssignment.ProjectSkills.Add(projectSkill);
        }
        
        await DbContext.ProjectAssignments.AddAsync(projectAssignment);
        await DbContext.SaveChangesAsync();
        
        // Act
        var result = await _projectAssignmentService.DeleteAsync(projectAssignment.Id);
        
        // Assert
        Assert.True(result);
        
        // Verify it was removed from database
        var dbProjectAssignment = await DbContext.ProjectAssignments.FindAsync(projectAssignment.Id);
        Assert.Null(dbProjectAssignment);
        
        // Verify project skills were removed
        var projectSkills = DbContext.ProjectSkills.Where(ps => ps.ProjectAssignmentId == projectAssignment.Id).ToList();
        Assert.Empty(projectSkills);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Deleting project assignment with ID: {projectAssignment.Id}", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            "Project assignment deleted successfully:", Times.Once());
    }
    
    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        var result = await _projectAssignmentService.DeleteAsync(id);
        
        // Assert
        Assert.False(result);
        
        // Verify logging
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Information, 
            $"Deleting project assignment with ID: {id}", Times.Once());
        MockLoggerFactory.VerifyLog(_loggerMock, LogLevel.Warning, 
            $"Project assignment with ID {id} not found", Times.Once());
    }
}
