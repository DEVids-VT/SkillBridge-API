namespace SkillBridge.Services
{
    /// <summary>
    /// Provides methods for acquiring authentication tokens
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Gets a valid access token for the specified API
        /// </summary>
        /// <returns>A valid access token</returns>
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}
