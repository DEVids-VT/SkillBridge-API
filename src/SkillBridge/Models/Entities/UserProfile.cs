using System;

namespace SkillBridge.Models.Entities;

/// <summary>
/// Represents a user profile in the system, linked to an Auth0 user account
/// </summary>
public class UserProfile
{
    /// <summary>
    /// Gets or sets the ID of the user profile (Auth0 user ID)
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the date when this user profile was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the date when this user profile was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of user project assignments for this user
    /// </summary>
    public ICollection<UserProjectAssignment> UserProjectAssignments { get; set; } = new List<UserProjectAssignment>();
}