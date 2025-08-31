using NJsonSchema.Annotations;
using System;

namespace SkillBridge.Models.Entities
{
    /// <summary>
    /// Represents a task that belongs to a project assignment
    /// </summary>
    public class AssignmentTask
    {
        /// <summary>
        /// Gets or sets the unique identifier for the task
        /// </summary>
        [JsonSchemaIgnore]
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
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Gets or sets the sequence number of the task (for ordering)
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Gets or sets the ID of the project assignment this task belongs to
        /// </summary>
        [JsonSchemaIgnore]
        public Guid ProjectAssignmentId { get; set; }

        /// <summary>
        /// Gets or sets the project assignment this task belongs to
        /// </summary>
        [JsonSchemaIgnore]
        public ProjectAssignment? ProjectAssignment { get; set; }

        /// <summary>
        /// Gets or sets the date when this task was created
        /// </summary>
        [JsonSchemaIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date when this task was last updated
        /// </summary>
        [JsonSchemaIgnore]
        public DateTime? UpdatedAt { get; set; }
    }
}