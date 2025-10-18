using NJsonSchema.Annotations;

namespace SkillBridge.Models.Entities
{
    public class UserProjectAssignmentTask
    {
        [JsonSchemaIgnore] public Guid AssignmentTaskId { get; set; }
        [JsonSchemaIgnore] public AssignmentTask? AssignmentTask { get; set; }

        [JsonSchemaIgnore] public Guid ProjectAssignmentId { get; set; }
        [JsonSchemaIgnore] public string UserProfileId { get; set; } = default!;

        // principal: the row in user_project_assignments
        [JsonSchemaIgnore] public UserProjectAssignment? UserProjectAssignment { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}
