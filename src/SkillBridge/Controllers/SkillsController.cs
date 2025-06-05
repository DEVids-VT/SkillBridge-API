using Microsoft.AspNetCore.Mvc;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.Skill;

namespace SkillBridge.Controllers;

/// <summary>
/// Controller for skill operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillsController"/> class.
    /// </summary>
    /// <param name="skillService">The skill service</param>
    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    /// <summary>
    /// Creates a new skill
    /// </summary>
    /// <param name="request">The skill creation request</param>
    /// <returns>The created skill</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SkillResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSkillRequest request)
    {
        var skill = await _skillService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = skill.Id }, skill);
    }    /// <summary>
    /// Gets a skill by ID
    /// </summary>
    /// <param name="id">The skill ID</param>
    /// <returns>The skill if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SkillResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var skill = await _skillService.GetByIdAsync(id);
        return Ok(skill);
    }

    /// <summary>
    /// Gets all skills
    /// </summary>
    /// <returns>A list of all skills</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SkillResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var skills = await _skillService.GetAllAsync();
        return Ok(skills);
    }    /// <summary>
    /// Updates a skill
    /// </summary>
    /// <param name="id">The skill ID</param>
    /// <param name="request">The skill update request</param>
    /// <returns>The updated skill</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SkillResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSkillRequest request)
    {
        var skill = await _skillService.UpdateAsync(id, request);
        return Ok(skill);
    }    /// <summary>
    /// Deletes a skill
    /// </summary>
    /// <param name="id">The skill ID</param>
    /// <returns>No content if deleted successfully</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _skillService.DeleteAsync(id);
        return NoContent();
    }
}
