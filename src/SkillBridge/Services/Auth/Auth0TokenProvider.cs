using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SkillBridge.Infrastructure.Configuration;
using SkillBridge.Infrastructure.Exceptions;
using System.Text;

namespace SkillBridge.Services.Auth;

/// <summary>
/// Implementation of the token provider for Auth0
/// </summary>
public class Auth0TokenProvider : ITokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly Auth0Settings _options;
    private string? _cachedToken;
    private DateTime _tokenExpiration = DateTime.MinValue;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="Auth0TokenProvider"/> class.
    /// </summary>
    public Auth0TokenProvider(HttpClient httpClient, IOptions<Auth0Settings> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <summary>
    /// Gets a valid access token for Auth0
    /// </summary>
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        // Check if we have a valid cached token
        if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiration > DateTime.UtcNow.AddMinutes(5))
        {
            return _cachedToken;
        }

        // Use semaphore to prevent multiple token requests
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring the lock
            if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiration > DateTime.UtcNow.AddMinutes(5))
            {
                return _cachedToken;
            }

            // Prepare token request
            var tokenRequest = new
            {
                client_id = _options.ClientId,
                client_secret = _options.ClientSecret,
                audience = "https://dev-bnsuhx4qupwjsv3z.us.auth0.com/api/v2/",
                grant_type = "client_credentials"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(tokenRequest),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response;
            
            try
            {
                response = await _httpClient.PostAsync(
                    $"https://{_options.Domain}/oauth/token",
                    content,
                    cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                throw new ExternalServiceException("Auth0", "Failed to connect to Auth0 token endpoint", ex);
            }

            string responseContent;
            try
            {
                response.EnsureSuccessStatusCode();
                responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                throw new ExternalServiceException("Auth0", $"Auth0 returned an error: {response.StatusCode}", ex);
            }

            TokenResponse? tokenResponse;
            try
            {
                tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
            }
            catch (JsonException ex)
            {
                throw new ExternalServiceException("Auth0", "Failed to deserialize Auth0 token response", ex);
            }

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new ExternalServiceException("Auth0", "Auth0 returned an invalid token response");
            }

            // Cache the token
            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

            return _cachedToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = default!;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = default!;
    }
}
