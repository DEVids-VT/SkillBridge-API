
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.UserProfile
{
    public interface IUserProfileService
    {
        Task<UserProfileResponse> GetMyProfileAsync(string? userId = null);
        Task<UserProfileResponse> CreateAsync(CreateUserProfileRequest request, string? userId = null);
        Task<UserProfileResponse> UpdateAsync(UpdateUserProfileRequest request,string? userId);
        Task<UserProfileResponse> UpdateProfilePicture(ProfilePictureRequest request, string? userId);
        Task<UserProfileResponse> UpdateCVUpload(CVUploadRequest request, string? userId);
    }
}
