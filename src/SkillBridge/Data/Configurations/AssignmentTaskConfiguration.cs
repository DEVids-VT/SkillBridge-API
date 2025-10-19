using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Infrastructure.Validation;
using SkillBridge.Models.Entities;

namespace SkillBridge.Data.Configurations;

/// <summary>
/// Configuration for the AssignmentTask entity
/// </summary>
public class AssignmentTaskConfiguration : IEntityTypeConfiguration<AssignmentTask>
{
    /// <summary>
    /// Configures the entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<AssignmentTask> builder)
    {
        builder.ToTable("assignment_tasks");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(ValidationConstants.AssignmentTask.TitleMaxLength);
            
        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(ValidationConstants.AssignmentTask.DescriptionMaxLength);
            
        builder.Property(e => e.Sequence)
            .HasColumnName("sequence")
            .IsRequired();
            
        builder.Property(e => e.ProjectAssignmentId)
            .HasColumnName("project_assignment_id")
            .IsRequired();
            
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");
        
        // Configure relationship with ProjectAssignment
        builder.HasOne(e => e.ProjectAssignment)
              .WithMany(p => p.Tasks)
              .HasForeignKey(e => e.ProjectAssignmentId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.UserProjectAssignmentTasks)
               .WithOne(uat => uat.AssignmentTask)
               .HasForeignKey(uat => uat.AssignmentTaskId)
               .OnDelete(DeleteBehavior.Cascade);

        // Create indexes for common queries
        builder.HasIndex(e => e.ProjectAssignmentId);
        builder.HasIndex(e => e.Sequence);
    }
}