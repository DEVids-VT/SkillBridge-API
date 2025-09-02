using SkillBridge.Models.Enums;

namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for updating a project assignment
/// </summary>
public class UpdateProjectAssignmentRequest
{
    /// <summary>
    /// Gets or sets the title of the project assignment
    /// </summary>
    public string Title { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the description of the project assignment
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets a brief summary of the project assignment
    /// </summary>
    public string Summary { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the learning benefits that candidates can gain from this project assignment
    /// </summary>
    public string LearningBenefits { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the suggested approach for completing this project assignment
    /// </summary>
    public string SuggestedApproach { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the difficulty level of this project assignment
    /// </summary>
    public ProjectAssignmentLevel Level { get; set; }
    
    /// <summary>
    /// Gets or sets the deadline for the project assignment
    /// </summary>
    public DateTime Deadline { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the project assignment
    /// </summary>
    public ProjectAssignmentStatus Status { get; set; }
    
    /// <summary>
    /// Gets or sets the skill IDs associated with this project assignment
    /// </summary>
    public List<Guid> SkillIds { get; set; } = new List<Guid>();
}
