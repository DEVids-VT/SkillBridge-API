namespace SkillBridge.Infrastructure.Configuration;

/// <summary>
/// Represents Auth0 configuration settings for authentication and authorization.
/// </summary>
public class Auth0Settings
{
    /// <summary>
    /// The Auth0 domain (e.g., "your-tenant.auth0.com").
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// The identifier for the API registered in Auth0.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// The Client ID for the application registered in Auth0.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The Client Secret for the application registered in Auth0.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}