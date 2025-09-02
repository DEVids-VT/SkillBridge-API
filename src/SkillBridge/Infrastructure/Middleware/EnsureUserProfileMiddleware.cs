using Auth0.ManagementApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Models.Entities;
using SkillBridge.Services.CurrentUser;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Middleware;

/// <summary>
/// Middleware to ensure that authenticated users have a corresponding UserProfile in the database
/// </summary>
public class EnsureUserProfileMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EnsureUserProfileMiddleware> _logger;
    private readonly ManagementApiClient _managementApiClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnsureUserProfileMiddleware"/> class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    public EnsureUserProfileMiddleware(
        RequestDelegate next,
        ILogger<EnsureUserProfileMiddleware> logger,
        ManagementApiClient managementApiClient)
    {
        _next = next;
        _logger = logger;
        _managementApiClient = managementApiClient;
    }

    /// <summary>
    /// Processes the request
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="dbContext">The database context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        // Only process for authenticated users
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = GetUserId(context.User);

            if (!string.IsNullOrEmpty(userId))
            {
                var userProfileExists = await dbContext.UserProfiles
                    .AnyAsync(up => up.Id == userId);

                bool userRoleIsCandidate = await IsCandidate(userId);

                if (!userProfileExists && userRoleIsCandidate)
                {
                    _logger.LogInformation("Creating new user profile for user {UserId}", userId);

                    var userProfile = new UserProfile
                    {
                        Id = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await dbContext.UserProfiles.AddAsync(userProfile);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation("User profile created successfully for {UserId}", userId);
                }
            }
        }

        // Continue the pipeline
        await _next(context);
    }

    private static string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirst("sub")?.Value ??
               user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }

    public async Task<bool> IsCandidate(string userId)
    {
        var role = await _managementApiClient.Users.GetRolesAsync(userId);
        return role.Any(r => r.Name == "Candidate");

    }
}