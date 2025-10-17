using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Models.Entities;

namespace SkillBridge.Data.Configurations;

/// <summary>
/// Configuration for the UserProjectAssignment entity
/// </summary>
public class UserProjectAssignmentConfiguration : IEntityTypeConfiguration<UserProjectAssignment>
{
    /// <summary>
    /// Configures the entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<UserProjectAssignment> builder)
    {
        builder.ToTable("user_project_assignments");
        
        // Composite primary key
        builder.HasKey(e => new { e.UserProfileId, e.ProjectAssignmentId });
        
        // UserProfile relationship
        builder.HasOne(e => e.UserProfile)
            .WithMany(e => e.UserProjectAssignments)
            .HasForeignKey(e => e.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // ProjectAssignment relationship
        builder.HasOne(e => e.ProjectAssignment)
            .WithMany(e => e.UserProjectAssignments)
            .HasForeignKey(e => e.ProjectAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Properties
        builder.Property(e => e.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();
            
        builder.Property(e => e.ProjectAssignmentId)
            .HasColumnName("project_assignment_id")
            .IsRequired();
            
        builder.Property(e => e.ClaimedAt)
            .HasColumnName("claimed_at")
            .IsRequired();
            
        builder.Property(e => e.IsCompleted)
            .HasColumnName("is_completed")
            .IsRequired();
            
        builder.Property(e => e.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(e => e.Deadline)
            .HasColumnName("deadline")
            .IsRequired();

        builder.Property(e => e.TotalTasks)
            .HasColumnName("total_tasks")
            .IsRequired();

        builder.Property(e => e.CompletedTasks)
            .HasColumnName("completed_tasks")
            .IsRequired();

        builder.HasMany(e => e.UserAssignmentTasks)
               .WithOne(uat => uat.UserProjectAssignment)
               .HasForeignKey(uat => new { uat.UserProfileId, uat.ProjectAssignmentId })
               .HasPrincipalKey(e => new { e.UserProfileId, e.ProjectAssignmentId })
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.UserProfileId);
        builder.HasIndex(e => e.ProjectAssignmentId);
    }
}