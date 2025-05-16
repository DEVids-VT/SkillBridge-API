using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Models.Entities;

namespace SkillBridge.Data.Configurations;

/// <summary>
/// Configuration for the Company entity
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    /// <summary>
    /// Configures the entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");
        
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
            
        builder.Property(e => e.Auth0UserId)
            .HasColumnName("auth0_user_id")
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");
        
        // Create index on Auth0UserId for faster lookups when finding a company by user ID
        builder.HasIndex(e => e.Auth0UserId).IsUnique();
    }
}