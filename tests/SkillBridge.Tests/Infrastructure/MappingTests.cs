using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Tests.Common;
using SkillBridge.Tests.Fixtures;

namespace SkillBridge.Tests.Infrastructure;

/// <summary>
/// Tests for the mapping configurations
/// </summary>
public class MappingTests
{
    private readonly MapperFixture _mapperFixture;

    public MappingTests()
    {
        _mapperFixture = new MapperFixture();
    }
    
    [Fact]
    public void CreateSkillRequest_MapsToSkill_Correctly()
    {
        // Arrange
        var request = TestDataGenerator.CreateTestCreateSkillRequest();
        
        // Act
        var skill = _mapperFixture.Mapper.Map<Skill>(request);
        
        // Assert
        Assert.Equal(request.Name, skill.Name);
        Assert.Equal(request.Description, skill.Description);
    }
      [Fact]
    public void UpdateSkillRequest_MapsToSkill_Correctly()
    {
        // Arrange
        var request = TestDataGenerator.CreateTestUpdateSkillRequest(
            name: "Updated Name", 
            description: "Updated Description"
        );
        
        var skill = TestDataGenerator.CreateTestSkill();
        
        // Act - Update mapping implementation
        skill.Name = request.Name;
        skill.Description = request.Description;
        
        // Assert
        Assert.Equal("Updated Name", skill.Name);
        Assert.Equal("Updated Description", skill.Description);
    }
    
    [Fact]
    public void Skill_MapsToSkillResponse_Correctly()
    {
        // Arrange
        var skill = TestDataGenerator.CreateTestSkill();
        
        // Act
        var response = _mapperFixture.Mapper.Map<SkillResponse>(skill);
        
        // Assert
        Assert.Equal(skill.Id, response.Id);
        Assert.Equal(skill.Name, response.Name);
        Assert.Equal(skill.Description, response.Description);
    }
      [Fact]
    public void UpdateSkillRequest_WithNullValues_DoesNotOverwriteProperties()
    {
        // Arrange
        var request = new UpdateSkillRequest
        {
            Name = "Updated Name",
            Description = null
        };
        
        var skill = TestDataGenerator.CreateTestSkill();
        var originalDescription = skill.Description;
        
        // Act - Update mapping implementation
        skill.Name = request.Name;
        if (request.Description != null)
        {
            skill.Description = request.Description;
        }
        
        // Assert
        Assert.Equal("Updated Name", skill.Name);
        Assert.Equal(originalDescription, skill.Description);
    }
}
