namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for updating an assignment task
/// </summary>
public class UpdateAssignmentTaskRequest
{
    /// <summary>
    /// Gets or sets the title of the task
    /// </summary>
    public string Title { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the description of the task
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets whether the task is completed
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Gets or sets the sequence number of the task (for ordering)
    /// </summary>
    public int Sequence { get; set; }
}