using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Models.Entities;

namespace SkillBridge.Data.Configurations
{
    public class UserAssignmentTaskConfiguration : IEntityTypeConfiguration<UserAssignmentTask>
    {
        public void Configure(EntityTypeBuilder<UserAssignmentTask> builder)
        {
            builder.ToTable("user_assignment_tasks");

            // Composite PK: (user_profile_id, project_assignment_id, assignment_task_id)
            builder.HasKey(e => new { e.UserProfileId, e.ProjectAssignmentId, e.AssignmentTaskId });

            // Columns
            builder.Property(e => e.UserProfileId)
                   .HasColumnName("user_profile_id");

            builder.Property(e => e.ProjectAssignmentId)
                   .HasColumnName("project_assignment_id");

            builder.Property(e => e.AssignmentTaskId)
                   .HasColumnName("assignment_task_id");

            builder.Property(e => e.IsCompleted)
                   .HasColumnName("is_completed")
                   .HasDefaultValue(false);

            // Relationships
            builder.HasOne(e => e.UserProjectAssignment)
                   .WithMany(upa => upa.UserAssignmentTasks)
                   .HasForeignKey(e => new { e.UserProfileId, e.ProjectAssignmentId })
                   .HasPrincipalKey(upa => new { upa.UserProfileId, upa.ProjectAssignmentId })
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.AssignmentTask)
                   .WithMany(t => t.UserAssignmentTasks)
                   .HasForeignKey(e => e.AssignmentTaskId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Optional helpful indexes (keep if your queries use them)
            builder.HasIndex(e => e.AssignmentTaskId)
                   .HasDatabaseName("ix_user_assignment_tasks_assignment_task_id");

            builder.HasIndex(e => new { e.UserProfileId, e.ProjectAssignmentId })
                   .HasDatabaseName("ix_user_assignment_tasks_user_project");
        }
    }
}
