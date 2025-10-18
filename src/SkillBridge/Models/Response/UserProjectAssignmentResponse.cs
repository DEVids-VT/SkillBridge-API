using System;
using SkillBridge.Models.Response;

namespace SkillBridge.Models.Response;

/// <summary>
/// Response model for a user's project assignment
/// </summary>
public class UserProjectAssignmentResponse
{
    /// <summary>
    /// Gets or sets the project assignment
    /// </summary>
    public ProjectAssignmentResponse ProjectAssignment { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the date when the project was claimed by the user
    /// </summary>
    public DateTime ClaimedAt { get; set; }
    
    /// <summary>
    /// Gets or sets whether the project is completed by the user
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the project was completed by the user
    /// </summary>
    public DateTime? CompletedAt { get; set; }
     
    public DateTime Deadline { get; set; }

    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public double CompletionPercentage 
        => TotalTasks == 0 ? 0 : (double)CompletedTasks / TotalTasks * 100;
}