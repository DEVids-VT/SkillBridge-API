using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Services.UserRole;

namespace SkillBridge.Controllers;

/// <summary>
/// Controller for managing user roles
/// </summary>
[ApiController]
[Route("api/r")]
[Authorize]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRolesController"/> class.
    /// </summary>
    /// <param name="userRoleService">The user role service</param>
    public UserRolesController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }
    
    /// <summary>
    /// Assigns the Company role to the current user
    /// </summary>
    /// <returns>A result indicating success or failure</returns>
    [HttpPost("become-company")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> BecomeCompany()
    {
        var result = await _userRoleService.BecomeCompanyAsync();
        
        if (result)
        {
            return Ok(new { message = "Company role assigned successfully" });
        }
        
        return BadRequest(new { message = "Failed to assign Company role" });
    }
    
    /// <summary>
    /// Assigns the Candidate role to the current user
    /// </summary>
    /// <returns>A result indicating success or failure</returns>
    [HttpPost("become-candidate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> BecomeCandidate()
    {
        var result = await _userRoleService.BecomeCandidateAsync();
        
        if (result)
        {
            return Ok(new { message = "Candidate role assigned successfully" });
        }
        
        return BadRequest(new { message = "Failed to assign Candidate role" });
    }
}
