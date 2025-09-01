using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.Company;
using SkillBridge.Services.CurrentUser;
using SkillBridge.Services.UserProfile;

namespace SkillBridge.Controllers
{
    [ApiController]
    [Route("api/u")]
    public class UserProfileController: ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompaniesController"/> class.
        /// </summary>
        /// <param name="companyService">The company service</param>
        /// <param name="currentUser">The current user service</param>
        public UserProfileController(IUserProfileService userProfileService, ICurrentUser currentUser)
        {
            _userProfileService = userProfileService;
            _currentUser = currentUser;
        }

        [HttpGet("my")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile([FromQuery] string? userId)
        {
            var userProfile = await _userProfileService.GetMyProfileAsync(userId);

            return Ok(userProfile);
        }

        [Authorize(Policy = "Candidate")]
        [HttpPut]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UpdateUserProfileRequest request, string? userId)
        {
            var userProfile = await _userProfileService.UpdateAsync(request, userId);

            return Ok(userProfile);
        }
        /// <summary>
        /// Gets a company by ID
        /// </summary>
        /// <param name="id">The company ID</param>
        /// <returns>The company if found</returns>
        [HttpGet("id/{id:guid}")]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string? userId)
        {
            var userProfile = await _userProfileService.GetByIdAsync(userId);

            return Ok(userProfile);
        }

        /// <summary>
        /// Deletes a company
        /// </summary>
        /// <param name="id">The company ID</param>
        /// <returns>No content if deleted successfully</returns>
        [Authorize(Policy = "CandidateScope")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string? id)
        {
            await _userProfileService.DeleteAsync(id);

            return NoContent();
        }
    }
}
