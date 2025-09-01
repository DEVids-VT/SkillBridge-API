using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
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
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;
        private readonly ManagementApiClient _managementApiClient;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileService"/> class.
        /// </summary>
        public UserProfileService(
            AppDbContext dbContext,
            ICurrentUser current,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            ManagementApiClient managementApiClient)
        {
            _dbContext = dbContext;
            _currentUser = current;
            _mapper = mapper;
            _logger = logger;
            _managementApiClient = managementApiClient;
        }

        public async Task<UserProfileResponse> DeleteAsync(string? userId = null)
        {
            var auth0UserId = userId ?? _currentUser.GetUserId();
            _logger.LogInformation("Deleting profile with ID: {UserProfileId}", auth0UserId);

            var userProfile = await _dbContext.UserProfiles.FindAsync(auth0UserId);

            if (userProfile == null)
            {
                _logger.LogWarning("Profile with ID {UserProfileId} not found", auth0UserId);
                throw new EntityNotFoundException(nameof(Models.Entities.UserProfile), auth0UserId);
            }

            _dbContext.UserProfiles.Remove(userProfile);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Profile deleted successfully: {UserProfileId}", auth0UserId);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }

        public async Task<UserProfileResponse> GetByIdAsync(string? userId = null)
        {
            var auth0UserId = userId ?? _currentUser.GetUserId();
            _logger.LogInformation("Retrieving profile with ID: {UserProfileId}", auth0UserId);

            var userProfile = await _dbContext.UserProfiles.FindAsync(auth0UserId);

            if (userProfile == null)
            {
                _logger.LogWarning("Profile with ID {ProfileId} not found", auth0UserId);
                throw new EntityNotFoundException(nameof(Models.Entities.UserProfile), auth0UserId);
            }
            var username = (await _managementApiClient.Users.GetAsync(userProfile.Id)).UserName;
           
            _logger.LogInformation("Pofile found: {PofileName}", username);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }

        public async Task<UserProfileResponse> GetMyProfileAsync(string? userId=null)
        {
            // If userId is not provided, use the current user's ID
            var auth0UserId = userId ?? _currentUser.GetUserId();

            _logger.LogInformation("Retrieving profile user ID: {Auth0UserId}", auth0UserId);

            var userProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(c => c.Id == auth0UserId);

            if (userProfile == null)
            {
                _logger.LogWarning("Profile for user ID {Auth0UserId} not found", auth0UserId);
                throw new EntityNotFoundException("Profile", $"for user {auth0UserId}",
                    $"No profile found for user ID {auth0UserId}");
            }
            var username = (await _managementApiClient.Users.GetAsync(userProfile.Id)).UserName;
            _logger.LogInformation("Profile found: {ProfileName}", username);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }

        public async Task<UserProfileResponse> UpdateAsync(UpdateUserProfileRequest request, string? userId = null)
        {
             var auth0UserId = userId ?? _currentUser.GetUserId();
            _logger.LogInformation("Updating profile with ID: {UserProfileId}", auth0UserId);
           
            var userProfile = await _dbContext.UserProfiles.FindAsync(auth0UserId);

            if (userProfile == null)
            {
                _logger.LogWarning("Profile with ID {ProfileId} not found", auth0UserId);
                throw new EntityNotFoundException(nameof(Models.Entities.UserProfile), auth0UserId);
            }

            _mapper.Map(request, userProfile);
            userProfile.UpdatedAt = DateTime.UtcNow;

            _dbContext.UserProfiles.Update(userProfile);
            await _dbContext.SaveChangesAsync();

            var username = (await _managementApiClient.Users.GetAsync(userProfile.Id)).UserName;

            _logger.LogInformation("Profile updated successfully: {ProfileName}", username);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }
    }
}
