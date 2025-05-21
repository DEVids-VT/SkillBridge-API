using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using SkillBridge.Infrastructure.Configuration;

namespace SkillBridge.Services
{
    public class Auth0TokenProvider : ITokenProvider
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _options;
        private string? _cachedToken;
        private DateTime _tokenExpiration = DateTime.MinValue;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public Auth0TokenProvider(HttpClient httpClient, IOptions<Auth0Settings> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

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
                    audience = _options.Audience,
                    grant_type = "client_credentials"
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(tokenRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    $"https://{_options.Domain}/oauth/token",
                    content,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    throw new InvalidOperationException("Failed to retrieve Auth0 access token.");
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
}
