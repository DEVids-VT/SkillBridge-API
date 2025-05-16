namespace SkillBridge.Models.Enums;

/// <summary>
/// Represents the status of a project assignment
/// </summary>
public enum ProjectAssignmentStatus
{
    /// <summary>
    /// The project assignment is in draft mode and not visible to candidates
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// The project assignment is published and visible to candidates
    /// </summary>
    Published = 1,
    
    /// <summary>
    /// The project assignment has been completed
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// The project assignment has been cancelled
    /// </summary>
    Cancelled = 3
}