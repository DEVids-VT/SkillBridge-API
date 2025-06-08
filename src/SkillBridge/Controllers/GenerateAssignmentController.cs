using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.GenerateAssignment;
using SkillBridge.Services.ProjectAssignment;
using SkillBridge.Services.Skill;

namespace SkillBridge.Controllers;

/// <summary>
/// Controller for AI-powered project assignment generation
/// </summary>
[ApiController]
[Route("api/g")]
public class GenerateAssignmentController : ControllerBase
{    private readonly IGenerateAssignmentService _generateAssignmentService;
    private readonly IProjectAssignmentService _projectAssignmentService;
    private readonly ISkillService _skillService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateAssignmentController"/> class.
    /// </summary>
    /// <param name="generateAssignmentService">The generate assignment service</param>
    /// <param name="projectAssignmentService">The project assignment service</param>
    /// <param name="skillService">The skill service</param>
    public GenerateAssignmentController(
        IGenerateAssignmentService generateAssignmentService,
        IProjectAssignmentService projectAssignmentService,
        ISkillService skillService)
    {
        _generateAssignmentService = generateAssignmentService;
        _projectAssignmentService = projectAssignmentService;
        _skillService = skillService;
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
        // Generate the assignment using AI
        var generatedAssignment = await _generateAssignmentService.GenerateAssignmentAsync(request);
          // Get all skills to find matching ones by name
        var allSkills = await _skillService.GetAllAsync();
        var skillIdsByName = allSkills.ToDictionary(
            s => s.Name.ToLowerInvariant(), 
            s => s.Id);
        
        // Match skill names from the request with existing skill IDs
        // Create skills that don't exist
        var matchingSkillIds = new List<Guid>();
        foreach (var skillName in request.RequiredSkills)
        {
            if (skillIdsByName.TryGetValue(skillName.ToLowerInvariant(), out var skillId))
            {
                matchingSkillIds.Add(skillId);
            }
            else
            {
                // Create the skill if it doesn't exist
                var createSkillRequest = new CreateSkillRequest { Name = skillName };
                var newSkill = await _skillService.CreateAsync(createSkillRequest);
                matchingSkillIds.Add(newSkill.Id);
            }
        }
        
        var createRequest = new CreateProjectAssignmentRequest
        {
            Title = generatedAssignment.Title,
            Description = generatedAssignment.Description,
            Deadline = generatedAssignment.Deadline,
            Status = generatedAssignment.Status,
            SkillIds = matchingSkillIds
        };
        
        // Save the generated assignment to database through the project assignment service
        var savedAssignment = await _projectAssignmentService.CreateAsync(companyId, createRequest);
        
        return CreatedAtAction(
            actionName: nameof(ProjectAssignmentsController.GetById),
            controllerName: "ProjectAssignments",
            routeValues: new { id = savedAssignment.Id },
            value: savedAssignment);
    }    
    /// <summary>
    /// Generates a draft project assignment without saving it
    /// </summary>
    /// <param name="request">The candidate requirements</param>
    /// <returns>The generated draft project assignment</returns>
    [HttpPost("draft")]
    [Authorize(Policy = "CompanyScope")]
    [ProducesResponseType(typeof(Models.Entities.ProjectAssignment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateDraft([FromBody] CandidateRequirementsRequest request)
    {
        try
        {
            // Generate the assignment without saving it
            var draftAssignment = await _generateAssignmentService.GenerateAssignmentAsync(request);
            
            // Return the generated assignment directly
            return Ok(draftAssignment);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to generate assignment: {ex.Message}");
        }
    }
}
