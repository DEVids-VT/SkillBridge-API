namespace Endorsify.Infrastructure.Configuration;

/// <summary>
/// Represents PostgreSQL configuration settings for database connections.
/// </summary>
public class PostgresSettings
{
    /// <summary>
    /// The connection string to the PostgreSQL database.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
    
    /// <summary>
    /// The Supabase URL.
    /// </summary>
    public string SupabaseUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// The Supabase API Key.
    /// </summary>
    public string SupabaseKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Controls whether database migrations are applied on startup.
    /// </summary>
    public bool EnableAutoMigrations { get; set; } = false;
}