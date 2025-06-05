namespace SkillBridge.Services.Auth;

/// <summary>
/// Provides methods for acquiring authentication tokens
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// Gets a valid access token for the specified API
    /// </summary>
    /// <returns>A valid access token</returns>
    /// <exception cref="SkillBridge.Infrastructure.Exceptions.ExternalServiceException">Thrown when token acquisition fails</exception>
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}
