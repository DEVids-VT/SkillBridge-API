
using Auth0.ManagementApi;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.Company;
using SkillBridge.Services.CurrentUser;

namespace SkillBridge.Services.UserProfile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;
        private readonly ManagementApiClient _managementApiClient;
        private readonly ICurrentUser _currentUser;

        public UserProfileService(
            AppDbContext dbContext,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            ManagementApiClient managementApiClient,
            ICurrentUser currentUser)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _managementApiClient = managementApiClient;
            _currentUser = currentUser;
        }
        public async Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserProfileResponse> GetMyProfileAsync(string? userId = null)
        {
            // If userId is not provided, use the current user's ID
            var auth0UserId = userId ?? _currentUser.GetUserId();

            _logger.LogInformation("Retrieving profile for user ID: {Auth0UserId}", auth0UserId);

            var userProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(c => c.Id == auth0UserId);

            var nickName = GetNickNameAsync(userProfile.Id);
            if (userProfile == null)
            {
                _logger.LogWarning("Profile for user ID {Auth0UserId} not found", auth0UserId);
                throw new EntityNotFoundException("Profile", $"for user {auth0UserId}",
                    $"No profile found for user ID {auth0UserId}");
            }

            _logger.LogInformation("Profile found: {UserProfileName}", nickName);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }


        public async Task<UserProfileResponse> UpdateAsync(Guid id, UpdateUserProfileRequest request)
        {
            _logger.LogInformation("Updating profile with ID: {UserProfileId}", id);

            var userProfile = await _dbContext.UserProfiles.FindAsync(id);

            if (userProfile == null)
            {
                _logger.LogWarning("Profile with ID {UserProfileId} not found", id);
                throw new EntityNotFoundException(nameof(Models.Entities.Company), id);
            }
            var nickName = GetNickNameAsync(userProfile.Id);

            _mapper.Map(request, userProfile);
            userProfile.UpdatedAt = DateTime.UtcNow;

            _dbContext.UserProfiles.Update(userProfile);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Profile updated successfully: {UserProfileName}", nickName);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }

        public async Task<UserProfileResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving profile with ID: {UserProfileId}", id);

            var userProfile = await _dbContext.UserProfiles.FindAsync(id);

            var nickName = GetNickNameAsync(userProfile.Id);
            if (userProfile == null)
            {
                _logger.LogWarning("Profile with ID {UserProfileId} not found", id);
                throw new EntityNotFoundException(nameof(Models.Entities.UserProfile), id);
            }

            _logger.LogInformation("Profile found: {UserProfileName}", nickName);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }

        private async Task<string?> GetNickNameAsync(string id)
        {
            try
            {
                var user = await _managementApiClient.Users.GetAsync(id);
                return user.NickName; // may be null, that’s fine now
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve nickname for user {UserId}", id);
                throw new ExternalServiceException("Auth0", $"Could not get nickname for {id}", ex);
            }
        }

    }
}
