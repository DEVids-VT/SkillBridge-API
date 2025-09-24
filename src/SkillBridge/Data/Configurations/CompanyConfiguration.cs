using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillBridge.Infrastructure.Validation;
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
            .HasMaxLength(ValidationConstants.Company.NameMaxLength);
            
        builder.Property(e => e.About)
            .HasColumnName("about")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.AboutMaxLength);

        builder.Property(e => e.LogoUrl)
            .HasColumnName("logo_url");
            
        builder.Property(e => e.BannerUrl)
            .HasColumnName("banner_url")
            .HasMaxLength(ValidationConstants.Company.BannerUrlMaxLength);
            
        builder.Property(e => e.Activities)
            .HasColumnName("activities")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.ActivitiesMaxLength);
            
        builder.Property(e => e.Sector)
            .HasColumnName("sector")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.SectorMaxLength);
            
        builder.Property(e => e.HeadOfficeLocation)
            .HasColumnName("head_office_location")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.HeadOfficeLocationMaxLength);
            
        builder.Property(e => e.Technologies)
            .HasColumnName("technologies")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.TechnologiesMaxLength);
            
        builder.Property(e => e.YearEstablished)
            .IsRequired()
            .HasColumnName("year_established");
            
        builder.Property(e => e.HasOfficesInBulgaria)
            .HasColumnName("has_offices_in_bulgaria")
            .IsRequired();
            
        builder.Property(e => e.BulgarianOfficeLocations)
            .HasColumnName("bulgarian_office_locations")
            .HasMaxLength(ValidationConstants.Company.BulgarianOfficeLocationsMaxLength);
            
        builder.Property(e => e.EmployeesInBulgaria)
            .HasColumnName("employees_in_bulgaria");
            
        builder.Property(e => e.EmployeesWorldwide)
            .HasColumnName("employees_worldwide")
            .IsRequired();
            
        builder.Property(e => e.WhyWorkWithUs)
            .HasColumnName("why_work_with_us")
            .HasMaxLength(ValidationConstants.Company.WhyWorkWithUsMaxLength);
            
        builder.Property(e => e.WebsiteUrl)
            .HasColumnName("website_url")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.WebsiteUrlMaxLength);
            
        builder.Property(e => e.ContactName)
            .HasColumnName("contact_name")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.ContactNameMaxLength);
            
        builder.Property(e => e.ContactEmail)
            .HasColumnName("contact_email")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.ContactEmailMaxLength);
            
        builder.Property(e => e.ContactPhone)
            .HasColumnName("contact_phone")
            .IsRequired()
            .HasMaxLength(ValidationConstants.Company.ContactPhoneMaxLength);
            
        builder.Property(e => e.Auth0UserId)
            .HasColumnName("auth0_user_id")
            .HasMaxLength(ValidationConstants.Company.Auth0UserIdMaxLength)
            .IsRequired();
            
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");
        
        // Create index on Auth0UserId for faster lookups when finding a company by user ID
        builder.HasIndex(e => e.Auth0UserId).IsUnique();
        
        // Create index on Name for faster searches
        builder.HasIndex(e => e.Name);
        
        // Create index on Sector for filtering
        builder.HasIndex(e => e.Sector);
    }
}