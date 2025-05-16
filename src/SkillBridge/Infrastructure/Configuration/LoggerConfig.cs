using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace SkillBridge.Infrastructure.Configuration;

/// <summary>
/// Provides configuration for Serilog.
/// </summary>
public static class LoggerConfig
{
    /// <summary>
    /// Configures and initializes the Serilog logger.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured logger.</returns>
    public static Serilog.ILogger ConfigureLogger(IConfiguration configuration)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}
