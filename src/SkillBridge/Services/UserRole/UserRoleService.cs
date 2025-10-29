using Microsoft.Extensions.Logging;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Services.CurrentUser;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SkillBridge.UnitTests")]
namespace SkillBridge.Services.UserRole
{
    /// <summary>
    /// Implementation of the user role service.
    /// </summary>
    public class UserRoleService : IUserRoleService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRolesClient _rolesClient;
        private readonly IUsersClient _usersClient;
        private readonly ILogger<UserRoleService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleService"/> class for production use.
        /// </summary>
        public UserRoleService(
            ICurrentUser currentUser,
            Auth0.ManagementApi.ManagementApiClient managementApiClient,    
            ILogger<UserRoleService> logger)
        {
            _currentUser = currentUser;
            _rolesClient = managementApiClient.Roles;
            _usersClient = managementApiClient.Users;
            _logger = logger;
        }

        /// <summary>
        /// Internal constructor for unit testing with direct client injection.
        /// </summary>
        internal UserRoleService(
            ICurrentUser currentUser,
            IRolesClient rolesClient,
            IUsersClient usersClient,
            ILogger<UserRoleService> logger)
        {
            _currentUser = currentUser;
            _rolesClient = rolesClient;
            _usersClient = usersClient;
            _logger = logger;
        }

        /// <summary>
        /// Assigns the Company role to a user.
        /// </summary>
        public async Task<bool> BecomeCompanyAsync(string? userId = null)
        {
            userId ??= _currentUser.GetUserId();
            return await AssignRoleAsync(userId, "Company");
        }

        /// <summary>
        /// Assigns the Candidate role to a user.
        /// </summary>
        public async Task<bool> BecomeCandidateAsync(string? userId = null)
        {
            userId ??= _currentUser.GetUserId();
            return await AssignRoleAsync(userId, "Candidate");
        }

        private async Task<bool> AssignRoleAsync(string userId, string roleName)
        {
            var roleId = await GetRoleIdAsync(roleName);
            if (string.IsNullOrEmpty(roleId))
            {
                _logger.LogError("Role '{RoleName}' not found in Auth0", roleName);
                throw new ExternalServiceException("Auth0", $"Role '{roleName}' not found in Auth0");
            }

            var assignRequest = new AssignRolesRequest { Roles = new[] { roleId } };

            try
            {
                await _usersClient.AssignRolesAsync(userId, assignRequest);
                _logger.LogInformation("Assigned role {RoleName} to user {UserId}", roleName, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", roleName, userId);
                throw new ExternalServiceException("Auth0", $"Failed to assign role '{roleName}' to user", ex);
            }
        }

        private async Task<string?> GetRoleIdAsync(string roleName)
        {
            try
            {
                var roles = await _rolesClient.GetAllAsync(new GetRolesRequest());

                var role = roles.FirstOrDefault(r =>
                    r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

                return role?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role ID for {RoleName}", roleName);
                throw new ExternalServiceException("Auth0", $"Failed to retrieve role information for '{roleName}'", ex);
            }
        }
    }
}
