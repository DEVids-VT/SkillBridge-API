using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SkillBridge.Services;

/// <summary>
/// Implementation of the current user service
/// </summary>
public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    /// <summary>
    /// Gets the Auth0 user ID (sub claim) of the current user
    /// </summary>
    public string GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or user ID claim is missing");
        }
        
        return userId;
    }
    
    /// <summary>
    /// Checks if the current user has the specified role
    /// </summary>
    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
    
    /// <summary>
    /// Gets all claims for the current user
    /// </summary>
    public IEnumerable<Claim> GetClaims()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();
    }
}
