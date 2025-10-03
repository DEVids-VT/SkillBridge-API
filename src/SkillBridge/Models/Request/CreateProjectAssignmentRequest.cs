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
    public ProjectAssignmentLevel Level { get; set; } = ProjectAssignmentLevel.Intermediate;
    
    /// <summary>
    /// Gets or sets the deadline for the project assignment
    /// </summary>
    public TimeSpan Duratoin { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the project assignment
    /// </summary>
    public ProjectAssignmentStatus Status { get; set; } = ProjectAssignmentStatus.Draft;
    
    /// <summary>
    /// Gets or sets the skill names associated with this project assignment
    /// </summary>
    public List<string> Skills { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets the tasks associated with this project assignment
    /// </summary>
    public List<CreateAssignmentTaskRequest> Tasks { get; set; } = new List<CreateAssignmentTaskRequest>();
}
