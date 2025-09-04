
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.UserProfile
{
    public interface IUserProfileService
    {
        Task<UserProfileResponse> GetAsync(string? userId);
        Task<UserProfileResponse> UpdateAsync(UpdateUserProfileRequest request,string? userId);
        Task<List<Models.Entities.UserProfile>> GetByAssigmentId(Guid id);

    }
}
