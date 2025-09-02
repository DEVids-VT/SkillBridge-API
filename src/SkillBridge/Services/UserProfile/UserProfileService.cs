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
using SkillBridge.Services.File;

namespace SkillBridge.Services.UserProfile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;
        private readonly ManagementApiClient _managementApiClient;
        private readonly IFileUploader _fileUploader;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileService"/> class.
        /// </summary>
        public UserProfileService(
            AppDbContext dbContext,
            ICurrentUser current,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            ManagementApiClient managementApiClient,
            IFileUploader fileUploader)
        {
            _dbContext = dbContext;
            _currentUser = current;
            _mapper = mapper;
            _logger = logger;
            _managementApiClient = managementApiClient;
            _fileUploader = fileUploader;
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

            var cvUrl = userProfile.CVUpload;
            var profilePicture = userProfile.ProfilePicture;

            // Delete user
            _dbContext.UserProfiles.Remove(userProfile);
            await _dbContext.SaveChangesAsync();

            // Delete files and images
            await _fileUploader.DeleteFileAsync(userProfile.ProfilePicture, Models.Enums.FileType.Image);
            await _fileUploader.DeleteFileAsync(userProfile.CVUpload, Models.Enums.FileType.CV);

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
            var response = _mapper.Map<UserProfileResponse>(userProfile);

            // Adding valid Urls to the files and images
            response.ProfilePicture = await _fileUploader.GetFileAsync(userProfile.ProfilePicture, Models.Enums.FileType.Image);
            response.CVUpload = await _fileUploader.GetFileAsync(userProfile.CVUpload, Models.Enums.FileType.CV);

            return response;
        }

        public async Task<UserProfileResponse> GetMyProfileAsync(string? userId = null)
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

            var response = _mapper.Map<UserProfileResponse>(userProfile);

            // Adding valid Urls to the files and images
            response.ProfilePicture = await _fileUploader.GetFileAsync(userProfile.ProfilePicture, Models.Enums.FileType.Image);
            response.CVUpload = await _fileUploader.GetFileAsync(userProfile.CVUpload, Models.Enums.FileType.CV);

            return response;
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

            // Update files and images
            if (!string.IsNullOrEmpty(userProfile.CVUpload))
            {
                await _fileUploader.DeleteFileAsync(userProfile.CVUpload, Models.Enums.FileType.CV);
            }
            userProfile.CVUpload = await _fileUploader.UploadFileAsync(request.CVUpload, Models.Enums.FileType.CV);
            if (!string.IsNullOrEmpty(userProfile.ProfilePicture))
            {
                await _fileUploader.DeleteFileAsync(userProfile.ProfilePicture, Models.Enums.FileType.Image);
            }
            userProfile.ProfilePicture = await _fileUploader.UploadFileAsync(request.ProfilePicture, Models.Enums.FileType.Image);

            userProfile.UpdatedAt = DateTime.UtcNow;

            _dbContext.UserProfiles.Update(userProfile);
            await _dbContext.SaveChangesAsync();

            var username = (await _managementApiClient.Users.GetAsync(userProfile.Id)).UserName;

            _logger.LogInformation("Profile updated successfully: {ProfileName}", username);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }
    }
}
