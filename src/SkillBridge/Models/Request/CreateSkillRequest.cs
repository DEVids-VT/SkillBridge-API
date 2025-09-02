namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for creating a new skill
/// </summary>
public class CreateSkillRequest
{
    /// <summary>
    /// Gets or sets the name of the skill
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the description of the skill
    /// </summary>
    public string? Description { get; set; }
}
