using System;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for claiming a project assignment
/// </summary>
public class ClaimProjectRequest
{
    /// <summary>
    /// Gets or sets the ID of the project assignment to claim
    /// </summary>
    [Required]
    public Guid ProjectAssignmentId { get; set; }
}