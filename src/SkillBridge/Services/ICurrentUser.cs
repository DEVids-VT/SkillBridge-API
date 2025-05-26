using System.Security.Claims;

namespace SkillBridge.Services;

/// <summary>
/// Interface for accessing the current authenticated user information
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the Auth0 user ID (sub claim) of the current user
    /// </summary>
    /// <returns>The Auth0 user ID</returns>
    string GetUserId();
    
    /// <summary>
    /// Checks if the current user has the specified role
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <returns>True if the user has the role, otherwise false</returns>
    bool IsInRole(string role);
    
    /// <summary>
    /// Gets all claims for the current user
    /// </summary>
    /// <returns>The current user's claims</returns>
    IEnumerable<Claim> GetClaims();
}
