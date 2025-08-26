using Microsoft.AspNetCore.Mvc;
using SkillBridge.Models.Request;
using SkillBridge.Services.Company;
using SkillBridge.Services.CurrentUser;
using SkillBridge.Services.UserProfile;

namespace SkillBridge.Controllers
{
    [ApiController]
    [Route("api/c")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ICurrentUser _currentUser;

        public UserProfileController(IUserProfileService candidateService, ICurrentUser currentUser)
        {
            _userProfileService = candidateService;
            _currentUser = currentUser;
        }


        public async Task<IActionResult> GetById(Guid id)
        {
            var company = await _userProfileService.GetByIdAsync(id);
            return Ok(company);
        }

        public async Task<IActionResult> GetMyProfile([FromQuery] string? userId = null)
        {
            var company = await _userProfileService.GetMyProfileAsync(userId);
            return Ok(company);
        }
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserProfileRequest request)
        {
            var company = await _userProfileService.UpdateAsync(id, request);
            return Ok(company);
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userProfileService.DeleteAsync(id);
            return NoContent();
        }
    }
}
