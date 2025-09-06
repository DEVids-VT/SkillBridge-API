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
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileController"/> class.
        /// </summary>
        /// <param name="companyService">The company service</param>
        /// <param name="currentUser">The current user service</param>
        public UserProfileController(IUserProfileService userProfileService, ICurrentUser currentUser)
        {
            _userProfileService = userProfileService;
            _currentUser = currentUser;
        }
        /// <summary>
        /// Gets a userProfile by ID
        /// </summary>
        /// <param name="id">The userProfile ID</param>
        /// <returns>The userProfile if found</returns>
        [HttpGet("{userId?}")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string? userId)
        {
            var userProfile = await _userProfileService.GetAsync(userId);

            return Ok(userProfile);
        }


        /// Updates a userProfile
        /// </summary>
        /// <param name="userId">The userProfile ID</param>
        /// <param name="request">The userProfile update request</param>
        /// <returns>The updated userProfile</returns>
        [Authorize(Policy = "Candidate")]
        [HttpPut("{userId?}")]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] string? userId,[FromBody] UpdateUserProfileRequest request)
        {
            var userProfile = await _userProfileService.UpdateAsync(request, userId);

            return Ok(userProfile);
        }



        [Authorize(Policy = "Candidate")]
        [HttpPatch("{userId?}/cv")]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CVUploadAsync([FromRoute] string? userId, [FromForm] CVUploadRequest request)
        {
            var userProfile = await _userProfileService.UpdateCVUpload(request, userId);
            return Ok(userProfile);
        }

        [Authorize(Policy = "Candidate")]
        [HttpPatch("{userId?}/profile-picture")]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProfilePictureAsync([FromRoute] string? userId, [FromForm] ProfilePictureRequest request)
        {
            var userProfile = await _userProfileService.UpdateProfilePicture(request, userId);
            return Ok(userProfile);
        }

    }
}
