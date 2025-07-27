using System;
using NJsonSchema.Annotations;

namespace SkillBridge.Models.Entities;

/// <summary>
/// Represents a many-to-many relationship between UserProfile and ProjectAssignment entities
/// </summary>
public class UserProjectAssignment
{
    /// <summary>
    /// Gets or sets the ID of the user profile
    /// </summary>
    [JsonSchemaIgnore]
    public string UserProfileId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user profile
    /// </summary>
    [JsonSchemaIgnore]
    public UserProfile? UserProfile { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the project assignment
    /// </summary>
    [JsonSchemaIgnore]
    public Guid ProjectAssignmentId { get; set; }
    
    /// <summary>
    /// Gets or sets the project assignment
    /// </summary>
    [JsonSchemaIgnore]
    public ProjectAssignment? ProjectAssignment { get; set; }
    
    /// <summary>
    /// Gets or sets the date when this project was claimed by the user
    /// </summary>
    public DateTime ClaimedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the completion status of the user's project assignment
    /// </summary>
    public bool IsCompleted { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the date when the user completed the project assignment
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}