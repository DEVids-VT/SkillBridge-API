
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.UserProfile
{
    public interface IUserProfileService
    {
        Task DeleteAsync(Guid id);
        Task<UserProfileResponse> GetByIdAsync(Guid id);
        Task<UserProfileResponse> UpdateAsync(Guid id, UpdateUserProfileRequest request);

        Task<UserProfileResponse> GetMyProfileAsync(string? userId);
    }
}
