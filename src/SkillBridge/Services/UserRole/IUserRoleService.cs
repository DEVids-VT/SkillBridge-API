    namespace SkillBridge.Services.UserRole;

/// <summary>
/// Provides methods for managing user roles
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Assigns the Company role to a user
    /// </summary>
    /// <param name="userId">The user ID, if not provided uses the current user</param>
    /// <returns>True if successful</returns>
    Task<bool> BecomeCompanyAsync(string? userId = null);
    
    /// <summary>
    /// Assigns the Candidate role to a user
    /// </summary>
    /// <param name="userId">The user ID, if not provided uses the current user</param>
    /// <returns>True if successful</returns>
    Task<bool> BecomeCandidateAsync(string? userId = null);
}
