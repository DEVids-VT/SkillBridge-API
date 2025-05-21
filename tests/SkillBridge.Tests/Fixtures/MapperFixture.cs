using Mapster;
using MapsterMapper;
using SkillBridge.Infrastructure.Mapping;

namespace SkillBridge.Tests.Fixtures;

/// <summary>
/// Fixture for creating and configuring Mapster mapping
/// </summary>
public class MapperFixture
{
    public IMapper Mapper { get; }    public MapperFixture()
    {
        // Create a new configuration instance, not using global settings
        var config = new TypeAdapterConfig();
        
        // Register all mapping configurations from the application
        new CompanyMappingConfig().Register(config);
        new ProjectAssignmentMappingConfig().Register(config);
        new SkillMappingConfig().Register(config);
        
        Mapper = new Mapper(config);
    }
}
