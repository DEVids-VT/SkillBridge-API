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
    public ProjectAssignmentStatus Status { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the company that created this project assignment
    /// </summary>
    public Guid CompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the company name that created this project assignment
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the skills required for this project assignment
    /// </summary>
    public List<SkillResponse> Skills { get; set; } = new List<SkillResponse>();
    
    /// <summary>
    /// Gets or sets the date when this project assignment was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date when this project assignment was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
