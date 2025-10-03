using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.UserProjectAssignment;
using System.Security.Claims;
using SkillBridge.Services.CurrentUser;

namespace SkillBridge.Controllers;

/// <summary>
/// Controller for user project assignment operations
/// </summary>
[ApiController]
[Authorize]
[Route("api/user/projects")]
public class UserProjectAssignmentsController : ControllerBase
{
    private readonly IUserProjectAssignmentService _userProjectAssignmentService;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProjectAssignmentsController"/> class.
    /// </summary>
    /// <param name="userProjectAssignmentService">The user project assignment service</param>
    public UserProjectAssignmentsController(IUserProjectAssignmentService userProjectAssignmentService, ICurrentUser currentUser)
    {
        _userProjectAssignmentService = userProjectAssignmentService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Claims a project assignment for the current user
    /// </summary>
    /// <param name="request">The claim project request</param>
    /// <returns>The claimed project assignment</returns>
    [HttpPost("claim")]
    [ProducesResponseType(typeof(UserProjectAssignmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClaimProject([FromBody] ClaimProjectRequest request)
    {
        var userId = _currentUser.GetUserId();

        var result = await _userProjectAssignmentService.ClaimProjectAsync(userId, request);
        return Ok(result);
    }

    /// <summary>
    /// Gets all project assignments claimed by the current user
    /// </summary>
    /// <returns>A list of project assignments claimed by the user</returns>
    [HttpGet("mine")]
    [ProducesResponseType(typeof(IEnumerable<UserProjectAssignmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserProjects()
    {
        var userId = _currentUser.GetUserId();

        var result = await _userProjectAssignmentService.GetUserProjectsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Marks a project assignment as completed by the current user
    /// </summary>
    /// <param name="request">The completion request containing project assignment details</param>
    /// <returns>The updated user project assignment</returns>
    [HttpPost("complete/{userId?}")]
    [ProducesResponseType(typeof(UserProjectAssignmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteProject([FromBody] CompleteUserProjectAssignmentRequest request, [FromQuery] string? userId)
    {
        //var userId = _currentUser.GetUserId();

        var result = await _userProjectAssignmentService.CompleteProjectAsync(userId, request);
        return Ok(result);
    }
}