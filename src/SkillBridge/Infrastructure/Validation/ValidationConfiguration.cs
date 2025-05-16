using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SkillBridge.Infrastructure.Validation;

/// <summary>
/// Provides extension methods for configuring validation in the application.
/// </summary>
public static class ValidationConfiguration
{
    /// <summary>
    /// Adds FluentValidation services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        // Register all validators from the assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Add FluentValidation to the MVC pipeline
        services.AddFluentValidationAutoValidation();
        
        return services;
    }
}
