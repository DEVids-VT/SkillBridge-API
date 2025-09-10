using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Models.Entities;

namespace SkillBridge.Data;

/// <summary>
/// Main database context for the application, providing access to all database entities.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the companies in the database
    /// </summary>
    public virtual DbSet<Company> Companies { get; set; } = null!;

    /// <summary>
    /// Gets or sets the project assignments in the database
    /// </summary>
    public virtual DbSet<ProjectAssignment> ProjectAssignments { get; set; } = null!;

    /// <summary>
    /// Gets or sets the skills in the database
    /// </summary>
    public virtual DbSet<Skill> Skills { get; set; } = null!;

    /// <summary>
    /// Gets or sets the project skills in the database
    /// </summary>
    public virtual DbSet<ProjectSkill> ProjectSkills { get; set; } = null!;

    /// <summary>
    /// Gets or sets the assignment tasks in the database
    /// </summary>
    public virtual DbSet<AssignmentTask> AssignmentTasks { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user profiles in the database
    /// </summary>
    public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the user project assignments in the database
    /// </summary>
    public virtual DbSet<UserProjectAssignment> UserProjectAssignments { get; set; } = null!;

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
        
        // Apply all IEntityTypeConfiguration<> implementations from the Configurations namespace
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}