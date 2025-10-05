using SkillBridge.Models.Enums;

namespace SkillBridge.Models.Response;

/// <summary>
/// Response model for project assignment data
/// </summary>
public class ProjectAssignmentResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the project assignment
    /// </summary>
    public Guid Id { get; set; }
    
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
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the project assignment
    /// </summary>
    public ProjectAssignmentStatus Status { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the company that created this project assignment
    /// </summary>
    public Guid CompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the company name that created this project assignment
    /// </summary>
    public string CompanyName { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the skills required for this project assignment
    /// </summary>
    public List<SkillResponse> Skills { get; set; } = new List<SkillResponse>();
    
    /// <summary>
    /// Gets or sets the tasks belonging to this project assignment
    /// </summary>
    public List<AssignmentTaskResponse> Tasks { get; set; } = new List<AssignmentTaskResponse>();
    
    /// <summary>
    /// Gets or sets the date when this project assignment was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date when this project assignment was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
