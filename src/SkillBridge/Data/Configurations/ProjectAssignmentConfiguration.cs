using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Infrastructure.Validation;
using SkillBridge.Models.Entities;

namespace SkillBridge.Data.Configurations;

/// <summary>
/// Configuration for the ProjectAssignment entity
/// </summary>
public class ProjectAssignmentConfiguration : IEntityTypeConfiguration<ProjectAssignment>
{
    /// <summary>
    /// Configures the entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<ProjectAssignment> builder)
    {
        builder.ToTable("project_assignments");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(ValidationConstants.ProjectAssignment.TitleMaxLength);

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(e => e.Summary)
            .HasColumnName("summary")
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.LearningBenefits)
            .HasColumnName("learning_benefits")
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.SuggestedApproach)
            .HasColumnName("suggested_approach")
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.Level)
            .HasColumnName("level")
            .IsRequired();
            
        builder.Property(e => e.Duration)
            .HasColumnName("duration")
            .IsRequired();
            
        builder.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired();
            
        builder.Property(e => e.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();
            
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");
        
        // Configure relationship with Company
        builder.HasOne(e => e.Company)
              .WithMany(c => c.ProjectAssignments)
              .HasForeignKey(e => e.CompanyId)
              .OnDelete(DeleteBehavior.Cascade);
        
        // Create indexes for common queries
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CompanyId);
        builder.HasIndex(e => e.Duration);
        builder.HasIndex(e => e.Level);
    }
}