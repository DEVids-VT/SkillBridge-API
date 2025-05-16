namespace SkillBridge.Models.Entities;

/// <summary>
/// Represents a company in the SkillBridge platform
/// </summary>
public class Company
{
    /// <summary>
    /// Gets or sets the unique identifier for the company
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the company
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the company
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the Auth0 user ID of the company owner
    /// </summary>
    public string Auth0UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the collection of project assignments created by this company
    /// </summary>
    public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
    
    /// <summary>
    /// Gets or sets the date when this company was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the date when this company was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}