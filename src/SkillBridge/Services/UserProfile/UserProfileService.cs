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
        private readonly IFileUploader _fileUploader;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileService"/> class.
        /// </summary>
        public UserProfileService(
            AppDbContext dbContext,
            ICurrentUser current,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            IFileUploader fileUploader
            )
        {
            _dbContext = dbContext;
            _currentUser = current;
            _mapper = mapper;
            _logger = logger;
            _fileUploader = fileUploader;
        }

        public async Task<UserProfileResponse> CreateAsync(CreateUserProfileRequest request, string? userId = null)
        {
            var auth0UserId = userId ?? _currentUser.GetUserId();
            _logger.LogInformation("Retrieving profile with ID: {UserProfileId}", auth0UserId);

            if (await _dbContext.UserProfiles.FindAsync(auth0UserId) != null)
            {
                _logger.LogWarning("Profile with ID {UserProfileId} already exists", auth0UserId);
                throw new InvalidOperationException($"A profile with ID '{auth0UserId}' already exists and cannot be created again.");
            }
            var userProfile = _mapper.Map<Models.Entities.UserProfile>(request);

            if (request.CVUpload != null)
                userProfile.CVUpload = await _fileUploader.UploadFileAsync(request.CVUpload, Models.Enums.FileType.CV);
            else
                userProfile.CVUpload = string.Empty;

            if (request.ProfilePicture != null)
                userProfile.ProfilePicture = await _fileUploader.UploadFileAsync(request.ProfilePicture, Models.Enums.FileType.Image);
            else
                userProfile.ProfilePicture = string.Empty;

            userProfile.Id = auth0UserId;

            await _dbContext.UserProfiles.AddAsync(userProfile);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created new UserProfile with ID: {UserProfileId}", auth0UserId);

            return _mapper.Map<UserProfileResponse>(userProfile);
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

            var response = _mapper.Map<UserProfileResponse>(userProfile);

            // Adding valid Urls to the files and images
            if (!string.IsNullOrEmpty(userProfile.ProfilePicture))
                response.ProfilePicture = await _fileUploader.GetFileAsync(userProfile.ProfilePicture, Models.Enums.FileType.Image);
            if (!string.IsNullOrEmpty(userProfile.CVUpload))
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

            if (request.GitHubConnection != null)
            {
                userProfile.GitHubConnection = request.GitHubConnection;
            }

            userProfile.UpdatedAt = DateTime.UtcNow;

            _dbContext.UserProfiles.Update(userProfile);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Profile updated successfully with Id: {ProfileId}", userProfile.Id);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }

        public async Task<UserProfileResponse> UpdateCVUpload(CVUploadRequest request, string? userId = null)
        {
            //throw new NotImplementedException();
            var auth0UserId = userId ?? _currentUser.GetUserId();
            _logger.LogInformation("Updating profile with ID: {UserProfileId}", auth0UserId);

            var userProfile = await _dbContext.UserProfiles.FindAsync(auth0UserId);

            if (userProfile == null)
            {
                _logger.LogWarning("Profile with ID {ProfileId} not found", auth0UserId);
                throw new EntityNotFoundException(nameof(Models.Entities.UserProfile), auth0UserId);
            }


            if (!string.IsNullOrEmpty(userProfile.CVUpload))
            {
                await _fileUploader.DeleteFileAsync(userProfile.CVUpload, Models.Enums.FileType.CV);
                userProfile.CVUpload = null;
            }
            if (request.CVUpload != null)
            {
                userProfile.CVUpload = await _fileUploader.UploadFileAsync(request.CVUpload, Models.Enums.FileType.CV);
            }

            userProfile.UpdatedAt = DateTime.UtcNow;

            _dbContext.UserProfiles.Update(userProfile);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Profile updated successfully with Id: {ProfileId}", userProfile.Id);

            return _mapper.Map<UserProfileResponse>(userProfile);

        }

        public async Task<UserProfileResponse> UpdateProfilePicture(ProfilePictureRequest request, string? userId = null)
        {
            var auth0UserId = userId ?? _currentUser.GetUserId();
            _logger.LogInformation("Updating profile with ID: {UserProfileId}", auth0UserId);

            var userProfile = await _dbContext.UserProfiles.FindAsync(auth0UserId);

            if (userProfile == null)
            {
                _logger.LogWarning("Profile with ID {ProfileId} not found", auth0UserId);
                throw new EntityNotFoundException(nameof(Models.Entities.UserProfile), auth0UserId);
            }


            if (!string.IsNullOrEmpty(userProfile.ProfilePicture))
            {
                await _fileUploader.DeleteFileAsync(userProfile.ProfilePicture, Models.Enums.FileType.Image);
                userProfile.ProfilePicture = null;
            }
            if (request.ProfilePicture != null)
            {
                userProfile.ProfilePicture = await _fileUploader.UploadFileAsync(request.ProfilePicture, Models.Enums.FileType.Image);
            }

            userProfile.UpdatedAt = DateTime.UtcNow;

            _dbContext.UserProfiles.Update(userProfile);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Profile updated successfully with Id: {ProfileId}", userProfile.Id);

            return _mapper.Map<UserProfileResponse>(userProfile);
        }
    }
}
 