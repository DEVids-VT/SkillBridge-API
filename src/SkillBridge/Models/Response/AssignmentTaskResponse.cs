using SkillBridge.Models.Entities;

namespace SkillBridge.Models.Response;

/// <summary>
/// Response model for assignment task data
/// </summary>
public class AssignmentTaskResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the task
    /// </summary>
    public Guid Id { get; set; }

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

    /// <summary>
    /// Gets or sets the ID of the project assignment this task belongs to
    /// </summary>
    public Guid ProjectAssignmentId { get; set; }

    /// <summary>
    /// Gets or sets the date when this task was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date when this task was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}