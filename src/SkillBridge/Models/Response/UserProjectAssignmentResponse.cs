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

    /// <summary>
    /// Gets or sets the deadline date for the project assignment
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// Gets or sets the URL of the submission repository containing the completed work.
    /// </summary>
    public string? SubmissionRepositoryUrl { get; set; }

    /// <summary>
    /// Gets or sets optional notes or comments related to the submission.
    /// </summary>
    public string? SubmissionNotes { get; set; }

    /// <summary>
    /// Gets or sets the date when the project submission URL was added.
    /// </summary>
    public DateTime? SubmittedAt { get; set; }
}