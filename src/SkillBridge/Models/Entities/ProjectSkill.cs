using NJsonSchema.Annotations;

namespace SkillBridge.Models.Entities
{

    /// <summary>
    /// Represents a many-to-many relationship between ProjectAssignment and Skill entities
    /// </summary>
    public class ProjectSkill
    {
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
        /// Gets or sets the ID of the skill
        /// </summary>
        [JsonSchemaIgnore]
        public Guid SkillId { get; set; }

        /// <summary>
        /// Gets or sets the skill
        /// </summary>
        public Skill? Skill { get; set; }
    }
}