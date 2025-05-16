namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for updating a skill
/// </summary>
public class UpdateSkillRequest
{
    /// <summary>
    /// Gets or sets the name of the skill
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the skill
    /// </summary>
    public string? Description { get; set; }
}
