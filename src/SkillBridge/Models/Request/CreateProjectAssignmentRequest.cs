using SkillBridge.Models.Enums;

namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for creating a new project assignment
/// </summary>
public class CreateProjectAssignmentRequest
{
    /// <summary>
    /// Gets or sets the title of the project assignment
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the project assignment
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the deadline for the project assignment
    /// </summary>
    public DateTime Deadline { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the project assignment
    /// </summary>
    public ProjectAssignmentStatus Status { get; set; } = ProjectAssignmentStatus.Draft;
    
    /// <summary>
    /// Gets or sets the skill IDs associated with this project assignment
    /// </summary>
    public List<Guid> SkillIds { get; set; } = new List<Guid>();
}
