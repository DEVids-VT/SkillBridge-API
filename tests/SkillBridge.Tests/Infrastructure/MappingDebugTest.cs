using Mapster;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Tests.Common;

namespace SkillBridge.Tests.Infrastructure;

/// <summary>
/// Debug tests for fixing mapping issues
/// </summary>
public class MappingDebugTest
{
    [Fact]
    public void DebugMapping_UpdateSkillRequest()
    {
        // Create a fresh TypeAdapterConfig instance
        var config = new TypeAdapterConfig();
        
        // Explicitly register the mapping
        config.NewConfig<UpdateSkillRequest, Skill>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description);
        
        // Create request and entity
        var request = new UpdateSkillRequest 
        { 
            Name = "Updated Name", 
            Description = "Updated Description" 
        };
        
        var skill = TestDataGenerator.CreateTestSkill();
        
        // Act - map using the explicit config
        skill = request.Adapt(skill, config);
        
        // Assert
        Assert.Equal("Updated Name", skill.Name);
        Assert.Equal("Updated Description", skill.Description);
    }
}
