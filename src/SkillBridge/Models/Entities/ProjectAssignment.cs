using NJsonSchema.Annotations;
using SkillBridge.Models.Enums;

namespace SkillBridge.Models.Entities
{
    /// <summary>
    /// Represents a project assignment created by a company
    /// </summary>
    public class ProjectAssignment
    {
        /// <summary>
        /// Gets or sets the unique identifier for the project assignment
        /// </summary>
        [JsonSchemaIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the project assignment
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the project assignment
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a brief summary of the project assignment
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the learning benefits that candidates can gain from this project assignment
        /// </summary>
        public string LearningBenefits { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the suggested approach for completing this project assignment
        /// </summary>
        public string SuggestedApproach { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the difficulty level of this project assignment
        /// </summary>
        public ProjectAssignmentLevel Level { get; set; } = ProjectAssignmentLevel.Intermediate;

        /// <summary>
        /// Gets or sets the deadline for the project assignment
        /// </summary>
        public DateTime Deadline { get; set; }

        /// <summary>
        /// Gets or sets the status of the project assignment
        /// </summary>
        public ProjectAssignmentStatus Status { get; set; } = ProjectAssignmentStatus.Draft;

        /// <summary>
        /// Gets or sets the ID of the company that created this project assignment
        /// </summary>
        [JsonSchemaIgnore]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the company that created this project assignment
        /// </summary>
        [JsonSchemaIgnore]
        public Company? Company { get; set; }

        /// <summary>
        /// Gets or sets the collection of skills required for this project assignment
        /// </summary>
        public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();

        /// <summary>
        /// Gets or sets the collection of tasks belonging to this project assignment
        /// </summary>
        public ICollection<AssignmentTask> Tasks { get; set; } = new List<AssignmentTask>();
        
        /// <summary>
        /// Gets or sets the collection of user project assignments for this project assignment
        /// </summary>
        [JsonSchemaIgnore]
        public ICollection<UserProjectAssignment> UserProjectAssignments { get; set; } = new List<UserProjectAssignment>();

        /// <summary>
        /// Gets or sets the date when this project assignment was created
        /// </summary>
        [JsonSchemaIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date when this project assignment was last updated
        /// </summary>
        [JsonSchemaIgnore]
        public DateTime? UpdatedAt { get; set; }
    }
}