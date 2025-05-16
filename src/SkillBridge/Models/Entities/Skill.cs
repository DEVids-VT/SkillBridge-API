namespace SkillBridge.Models.Entities;

/// <summary>
/// Represents a skill that can be associated with project assignments
/// </summary>
public class Skill
{
    /// <summary>
    /// Gets or sets the unique identifier for the skill
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the skill
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the skill
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of project skills associated with this skill
    /// </summary>
    public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
}