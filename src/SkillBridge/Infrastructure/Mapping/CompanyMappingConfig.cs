using Mapster;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Infrastructure.Mapping;

/// <summary>
/// Configures mappings for Company entities and DTOs
/// </summary>
public class CompanyMappingConfig : IRegister
{
    /// <summary>
    /// Configures the mapping between Company entities and DTOs
    /// </summary>
    /// <param name="config">The configuration to apply mappings to</param>
    public void Register(TypeAdapterConfig config)
    {
        // Entity to Response
        config.NewConfig<Company, CompanyResponse>();
            
        // Request to Entity
        config.NewConfig<CreateCompanyRequest, Company>()
            .Map(dest => dest.CreatedAt, _ => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, _ => (DateTime?)null);
            
        // Update Request to Entity
        config.NewConfig<UpdateCompanyRequest, Company>()
            .IgnoreNonMapped(true)
            .IgnoreNullValues(true)
            .Map(dest => dest.UpdatedAt, _ => DateTime.UtcNow)
            .Ignore(dest => dest.Auth0UserId)
            .Ignore(dest => dest.CreatedAt);
    }
}
