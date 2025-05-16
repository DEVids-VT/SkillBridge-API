using Mapster;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Infrastructure.Mapping;

/// <summary>
/// Configures mappings for Skill entities and DTOs
/// </summary>
public class SkillMappingConfig : IRegister
{
    /// <summary>
    /// Configures the mapping between Skill entities and DTOs
    /// </summary>
    /// <param name="config">The configuration to apply mappings to</param>
    public void Register(TypeAdapterConfig config)
    {
        // Entity to Response
        config.NewConfig<Skill, SkillResponse>();
            
        // Request to Entity
        config.NewConfig<CreateSkillRequest, Skill>();
            
        // Update Request to Entity
        config.NewConfig<UpdateSkillRequest, Skill>()
            .IgnoreNonMapped(true)
            .IgnoreNullValues(true);
    }
}
