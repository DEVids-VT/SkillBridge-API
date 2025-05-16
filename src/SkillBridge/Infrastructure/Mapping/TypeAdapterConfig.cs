using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace SkillBridge.Infrastructure.Mapping;

/// <summary>
/// Provides configuration for the Mapster type adapter.
/// </summary>
public static class MapsterConfiguration
{
    /// <summary>
    /// Registers and configures Mapster dependency injection in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        
        // Apply all entity to request/response mappings
        config.Scan(typeof(MapsterConfiguration).Assembly);
        
        // Register Mapster as a singleton
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        
        return services;
    }
}
