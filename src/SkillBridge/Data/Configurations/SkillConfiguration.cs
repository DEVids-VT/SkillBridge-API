using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Models.Entities;

namespace SkillBridge.Data.Configurations;

/// <summary>
/// Configuration for the Skill entity
/// </summary>
public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    /// <summary>
    /// Configures the entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skills");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(500);
        
        // Create unique index on Name to ensure skills are unique
        builder.HasIndex(e => e.Name).IsUnique();
    }
}