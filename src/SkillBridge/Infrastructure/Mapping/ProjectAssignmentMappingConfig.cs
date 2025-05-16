using Mapster;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Infrastructure.Mapping;

/// <summary>
/// Configures mappings for ProjectAssignment entities and DTOs
/// </summary>
public class ProjectAssignmentMappingConfig : IRegister
{
    /// <summary>
    /// Configures the mapping between ProjectAssignment entities and DTOs
    /// </summary>
    /// <param name="config">The configuration to apply mappings to</param>
    public void Register(TypeAdapterConfig config)
    {        // Entity to Response
        config.NewConfig<ProjectAssignment, ProjectAssignmentResponse>()
            .Map(dest => dest.CompanyName, src => src.Company != null ? src.Company.Name : string.Empty);
            
        // Request to Entity
        config.NewConfig<CreateProjectAssignmentRequest, ProjectAssignment>()
            .Ignore(dest => dest.ProjectSkills)
            .Ignore(dest => dest.Company)
            .Ignore(dest => dest.Id);
            
        // Update Request to Entity
        config.NewConfig<UpdateProjectAssignmentRequest, ProjectAssignment>()
            .Ignore(dest => dest.ProjectSkills)
            .Ignore(dest => dest.Company)
            .Ignore(dest => dest.Id)
            .IgnoreNullValues(true);
    }
}
