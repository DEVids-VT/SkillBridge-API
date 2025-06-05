using Microsoft.Extensions.Logging;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Services.CurrentUser;

namespace SkillBridge.Services.UserRole;

/// <summary>
/// Implementation of the user role service
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly ICurrentUser _currentUser;
    private readonly ManagementApiClient _managementApiClient;
    private readonly ILogger<UserRoleService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRoleService"/> class.
    /// </summary>
    public UserRoleService(
        ICurrentUser currentUser, 
        ManagementApiClient managementApiClient,
        ILogger<UserRoleService> logger)
    {
        _currentUser = currentUser;
        _managementApiClient = managementApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Assigns the Company role to a user
    /// </summary>
    public async Task<bool> BecomeCompanyAsync(string? userId = null)
    {
        userId ??= _currentUser.GetUserId();
        return await AssignRoleAsync(userId, "Company");
    }

    /// <summary>
    /// Assigns the Candidate role to a user
    /// </summary>
    public async Task<bool> BecomeCandidateAsync(string? userId = null)
    {
        userId ??= _currentUser.GetUserId();
        return await AssignRoleAsync(userId, "Candidate");
    }    protected virtual async Task<bool> AssignRoleAsync(string userId, string roleName)
    {
        try
        {
            // First, get the role ID by role name
            var roleId = await GetRoleIdAsync(roleName);
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ExternalServiceException("Auth0", $"Role '{roleName}' not found in Auth0");
            }

            // Assign the role to the user
            var assignRoleRequest = new AssignRolesRequest
            {
                Roles = new[] { roleId }
            };

            await _managementApiClient.Users.AssignRolesAsync(userId, assignRoleRequest);
            
            _logger.LogInformation("Successfully assigned role {Role} to user {UserId}", roleName, userId);
            return true;
        }
        catch (ExternalServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", roleName, userId);
            throw new ExternalServiceException("Auth0", $"Failed to assign role '{roleName}' to user", ex);
        }
    }
      protected virtual async Task<string?> GetRoleIdAsync(string roleName)
    {
        try
        {
            var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest());
            
            var role = roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            
            if (role == null)
            {
                _logger.LogError("Role '{RoleName}' not found in Auth0", roleName);
                return null;
            }

            return role.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role ID for {RoleName}", roleName);
            throw new ExternalServiceException("Auth0", $"Failed to retrieve role information for '{roleName}'", ex);
        }
    }
}
