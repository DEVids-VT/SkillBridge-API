
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.UserProfile
{
    public interface IUserProfileService
    {
        Task<UserProfileResponse> DeleteAsync(string? userId);
        Task<UserProfileResponse> CreateAsync(CreateUserProfileRequest request, string? userId);
        Task<UserProfileResponse> GetByIdAsync(string? userId);
        Task<UserProfileResponse> GetMyProfileAsync(string? userId);
        Task<UserProfileResponse> UpdateAsync(UpdateUserProfileRequest request, string? userId);
    }
}
