using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Models.Entities;

namespace SkillBridge.Data.Configurations;

/// <summary>
/// Configuration for the ProjectSkill entity
/// </summary>
public class ProjectSkillConfiguration : IEntityTypeConfiguration<ProjectSkill>
{
    /// <summary>
    /// Configures the entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<ProjectSkill> builder)
    {
        builder.ToTable("project_skills");
        
        builder.HasKey(e => new { e.ProjectAssignmentId, e.SkillId });
        
        builder.Property(e => e.ProjectAssignmentId)
            .HasColumnName("project_assignment_id");
            
        builder.Property(e => e.SkillId)
            .HasColumnName("skill_id");
        
        // Configure relationships
        builder.HasOne(e => e.ProjectAssignment)
              .WithMany(p => p.ProjectSkills)
              .HasForeignKey(e => e.ProjectAssignmentId)
              .OnDelete(DeleteBehavior.Cascade);
              
        builder.HasOne(e => e.Skill)
              .WithMany(s => s.ProjectSkills)
              .HasForeignKey(e => e.SkillId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}