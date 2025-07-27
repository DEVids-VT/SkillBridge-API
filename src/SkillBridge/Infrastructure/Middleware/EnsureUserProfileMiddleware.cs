using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Models.Entities;
using SkillBridge.Services.CurrentUser;

namespace SkillBridge.Infrastructure.Middleware;

/// <summary>
/// Middleware to ensure that authenticated users have a corresponding UserProfile in the database
/// </summary>
public class EnsureUserProfileMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EnsureUserProfileMiddleware> _logger;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnsureUserProfileMiddleware"/> class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    public EnsureUserProfileMiddleware(
        RequestDelegate next,
        ILogger<EnsureUserProfileMiddleware> logger,
        ICurrentUser currentUser)
    {
        _next = next;
        _logger = logger;
        _currentUser = currentUser;
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
            var userId = _currentUser.GetUserId();
            
            if (!string.IsNullOrEmpty(userId))
            {
                var userProfileExists = await dbContext.UserProfiles
                    .AnyAsync(up => up.Id == userId);
                
                if (!userProfileExists)
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
    
}