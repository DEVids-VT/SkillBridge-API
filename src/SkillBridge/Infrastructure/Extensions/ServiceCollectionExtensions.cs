using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Configuration;
using SkillBridge.Infrastructure.Mapping;
using SkillBridge.Infrastructure.Validation;
using SkillBridge.Services;
using System.Reflection;

namespace SkillBridge.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring services in the application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Auth0 authentication services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAuth0(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("Auth0").Get<Auth0Settings>()
            ?? throw new InvalidOperationException("Auth0 settings are not configured");
        
        services.Configure<Auth0Settings>(configuration.GetSection("Auth0"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{settings.Domain}/";
            options.Audience = settings.Audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        
        // Add authorization policies if needed
        services.AddAuthorization(options =>
        {
            // Example: options.AddPolicy("read:data", policy => policy.RequireClaim("scope", "read:data"));
        });

        return services;
    }

    /// <summary>
    /// Adds PostgreSQL database services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("Postgres").Get<PostgresSettings>()
            ?? throw new InvalidOperationException("Postgres settings are not configured");
        
        services.Configure<PostgresSettings>(configuration.GetSection("Postgres"));

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(settings.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(5);
            });
        });
        
        // Add additional database-related services here if needed
        // e.g., repositories, unit of work, etc.

        return services;
    }

    /// <summary>
    /// Adds Stripe payment processing services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddStripe(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("Stripe").Get<StripeSettings>()
            ?? throw new InvalidOperationException("Stripe settings are not configured");
        
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        
        // Configure Stripe API
        Stripe.StripeConfiguration.ApiKey = settings.SecretKey;
        
        // Register any Stripe-related services
        // Example: services.AddScoped<IPaymentService, StripePaymentService>();

        return services;
    }    /// <summary>
    /// Adds Web-related services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddMvc(o =>
        {
            o.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .Build()));
        });
        
        // Add validation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        
        // Add Mapster for object mapping
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(ServiceCollectionExtensions).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        
        // Register application services
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IProjectAssignmentService, ProjectAssignmentService>();
        
        return services;
    }
}