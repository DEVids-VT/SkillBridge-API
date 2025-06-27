using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.GenerateAssignment;

namespace SkillBridge.Controllers;

/// <summary>
/// Controller for AI-powered project assignment generation
/// </summary>
[ApiController]
[Route("api/g")]
public class GenerateAssignmentController : ControllerBase
{
    private readonly IGenerateAssignmentService _generateAssignmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateAssignmentController"/> class.
    /// </summary>
    /// <param name="generateAssignmentService">The generate assignment service</param>
    public GenerateAssignmentController(IGenerateAssignmentService generateAssignmentService)
    {
        _generateAssignmentService = generateAssignmentService;
    }

    /// <summary>
    /// Generates a project assignment based on candidate requirements
    /// </summary>
    /// <param name="companyId">The ID of the company creating the assignment</param>
    /// <param name="request">The candidate requirements</param>
    /// <returns>The generated project assignment</returns>
    [Authorize(Policy = "CompanyScope")]
    [HttpPost("{companyId}")]
    [ProducesResponseType(typeof(ProjectAssignmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]    
    public async Task<IActionResult> Generate(Guid companyId, [FromBody] CandidateRequirementsRequest request)
    {
        // Generate and save the assignment using the service
        var savedAssignment = await _generateAssignmentService.GenerateAndSaveAssignmentAsync(companyId, request);
        
        return CreatedAtAction(
            actionName: nameof(ProjectAssignmentsController.GetById),
            controllerName: "ProjectAssignments",
            routeValues: new { id = savedAssignment.Id },
            value: savedAssignment);
    }
}
