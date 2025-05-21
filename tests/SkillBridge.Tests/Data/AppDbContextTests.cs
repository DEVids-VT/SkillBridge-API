using Microsoft.EntityFrameworkCore;
using SkillBridge.Models.Entities;
using SkillBridge.Tests.Common;

namespace SkillBridge.Tests.Data;

/// <summary>
/// Tests for the <see cref="SkillBridge.Data.AppDbContext"/> class
/// </summary>
public class AppDbContextTests : DatabaseTestBase
{
    [Fact]
    public async Task Skills_CanBeAddedAndRetrieved()
    {
        // Arrange
        var skill = TestDataGenerator.CreateTestSkill(id: Guid.NewGuid());
        
        // Act
        await DbContext.Skills.AddAsync(skill);
        await DbContext.SaveChangesAsync();
        
        // Assert
        var retrievedSkill = await DbContext.Skills.FindAsync(skill.Id);
        Assert.NotNull(retrievedSkill);
        Assert.Equal(skill.Id, retrievedSkill.Id);
        Assert.Equal(skill.Name, retrievedSkill.Name);
        Assert.Equal(skill.Description, retrievedSkill.Description);
    }
    
    [Fact]
    public async Task Skills_CanBeUpdated()
    {
        // Arrange
        var skill = TestDataGenerator.CreateTestSkill(id: Guid.NewGuid());
        await DbContext.Skills.AddAsync(skill);
        await DbContext.SaveChangesAsync();
        
        // Act
        skill.Name = "Updated Name";
        skill.Description = "Updated Description";
        DbContext.Skills.Update(skill);
        await DbContext.SaveChangesAsync();
        
        // Assert
        var retrievedSkill = await DbContext.Skills.FindAsync(skill.Id);
        Assert.NotNull(retrievedSkill);
        Assert.Equal("Updated Name", retrievedSkill.Name);
        Assert.Equal("Updated Description", retrievedSkill.Description);
    }
    
    [Fact]
    public async Task Skills_CanBeDeleted()
    {
        // Arrange
        var skill = TestDataGenerator.CreateTestSkill(id: Guid.NewGuid());
        await DbContext.Skills.AddAsync(skill);
        await DbContext.SaveChangesAsync();
        
        // Act
        DbContext.Skills.Remove(skill);
        await DbContext.SaveChangesAsync();
        
        // Assert
        var retrievedSkill = await DbContext.Skills.FindAsync(skill.Id);
        Assert.Null(retrievedSkill);
    }
    
    [Fact]
    public async Task Skills_CanBeQueried()
    {
        // Arrange
        var skills = TestDataGenerator.CreateTestSkills(5);
        await DbContext.Skills.AddRangeAsync(skills);
        await DbContext.SaveChangesAsync();
        
        // Act
        var retrievedSkills = await DbContext.Skills.ToListAsync();
        
        // Assert
        Assert.Equal(5, retrievedSkills.Count);
    }
    
    [Fact]
    public async Task Skills_CanBeQueriedWithFilter()
    {
        // Arrange
        var skills = TestDataGenerator.CreateTestSkills(5);
        
        // Add a skill with a specific name
        skills.Add(new Skill
        {
            Id = Guid.NewGuid(),
            Name = "Special Skill",
            Description = "Special Description"
        });
        
        await DbContext.Skills.AddRangeAsync(skills);
        await DbContext.SaveChangesAsync();
        
        // Act
        var retrievedSkill = await DbContext.Skills
            .FirstOrDefaultAsync(s => s.Name == "Special Skill");
        
        // Assert
        Assert.NotNull(retrievedSkill);
        Assert.Equal("Special Skill", retrievedSkill.Name);
        Assert.Equal("Special Description", retrievedSkill.Description);
    }
      [Fact]
    public async Task ProjectSkills_CanBeAddedAndRetrieved()
    {
        // Arrange
        var skill = TestDataGenerator.CreateTestSkill(id: Guid.NewGuid());
        await DbContext.Skills.AddAsync(skill);
        
        var projectAssignmentId = Guid.NewGuid();
        
        var projectSkill = new ProjectSkill
        {
            SkillId = skill.Id,
            ProjectAssignmentId = projectAssignmentId
        };
        
        // Act
        await DbContext.ProjectSkills.AddAsync(projectSkill);
        await DbContext.SaveChangesAsync();
        
        // Assert
        var retrievedProjectSkill = await DbContext.ProjectSkills
            .FirstOrDefaultAsync(ps => ps.SkillId == skill.Id && ps.ProjectAssignmentId == projectAssignmentId);
        Assert.NotNull(retrievedProjectSkill);
        Assert.Equal(skill.Id, retrievedProjectSkill.SkillId);
        Assert.Equal(projectAssignmentId, retrievedProjectSkill.ProjectAssignmentId);
    }
}
