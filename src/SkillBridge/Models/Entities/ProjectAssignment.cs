
using SkillBridge.Models.Enums;

namespace SkillBridge.Models.Entities;

/// <summary>
/// Represents a project assignment created by a company
/// </summary>
public class ProjectAssignment
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
    public ProjectAssignmentStatus  Status { get; set; } = ProjectAssignmentStatus.Draft;
    
    /// <summary>
    /// Gets or sets the ID of the company that created this project assignment
    /// </summary>
    public Guid CompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the company that created this project assignment
    /// </summary>
    public Company? Company { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of skills required for this project assignment
    /// </summary>
    public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
    
    /// <summary>
    /// Gets or sets the date when this project assignment was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the date when this project assignment was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}