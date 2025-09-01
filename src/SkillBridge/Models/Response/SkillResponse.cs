namespace SkillBridge.Models.Response;

/// <summary>
/// Response model for skill data
/// </summary>
public class SkillResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the skill
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the skill
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the description of the skill
    /// </summary>
    public string? Description { get; set; }
}
