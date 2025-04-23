using Microsoft.EntityFrameworkCore;

namespace Endorsify.Data;

/// <summary>
/// Main database context for the application, providing access to all database entities.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the database model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure entity mappings and relationships here
        
        // Example:
        // modelBuilder.Entity<User>(entity =>
        // {
        //     entity.ToTable("users");
        //     entity.HasKey(e => e.Id);
        //     entity.Property(e => e.Id).HasColumnName("id");
        //     entity.Property(e => e.Email).HasColumnName("email").IsRequired();
        // });
    }
}