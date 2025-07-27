using Microsoft.AspNetCore.Builder;
using SkillBridge.Infrastructure.Middleware;

namespace SkillBridge.Infrastructure.Extensions;

/// <summary>
/// Extension methods for application middleware configuration
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the exception handler middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
    /// <summary>
    /// Adds middleware to ensure that authenticated users have a corresponding UserProfile in the database
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseEnsureUserProfile(this IApplicationBuilder app)
    {
        return app.UseMiddleware<EnsureUserProfileMiddleware>();
    }
}
