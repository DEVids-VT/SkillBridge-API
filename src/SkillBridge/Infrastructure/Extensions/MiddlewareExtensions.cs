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
}
