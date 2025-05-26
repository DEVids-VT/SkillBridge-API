using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using SkillBridge.Infrastructure.Configuration;

namespace SkillBridge.Services;

/// <summary>
/// Implementation of the user role service
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly ICurrentUser _currentUser;
    private readonly ITokenProvider _tokenProvider;
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserRoleService> _logger;
    private readonly Auth0Settings _auth0Settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRoleService"/> class.
    /// </summary>
    public UserRoleService(
        ICurrentUser currentUser, 
        ITokenProvider tokenProvider, 
        HttpClient httpClient,
        IOptions<Auth0Settings> auth0Settings,
        ILogger<UserRoleService> logger)
    {
        _currentUser = currentUser;
        _tokenProvider = tokenProvider;
        _httpClient = httpClient;
        _logger = logger;
        _auth0Settings = auth0Settings.Value;
    }

    /// <summary>
    /// Assigns the Company role to a user
    /// </summary>
    public async Task<bool> BecomeCompanyAsync(string? userId = null)
    {
        userId ??= _currentUser.GetUserId();
        return await AssignRoleAsync(userId, "Company");
    }

    /// <summary>
    /// Assigns the Candidate role to a user
    /// </summary>
    public async Task<bool> BecomeCandidateAsync(string? userId = null)
    {
        userId ??= _currentUser.GetUserId();
        return await AssignRoleAsync(userId, "Candidate");
    }

    private async Task<bool> AssignRoleAsync(string userId, string role)
    {
        try
        {
            var token = await _tokenProvider.GetTokenAsync();
            
            // Get the Management API URL using the domain from settings
            var apiUrl = $"https://{_auth0Settings.Domain}/api/v2/users/{Uri.EscapeDataString(userId)}/roles";
            
            // First, we need to find the role ID
            var roleId = await GetRoleIdAsync(role, token);
            if (string.IsNullOrEmpty(roleId))
            {
                _logger.LogError("Could not find the role ID for role: {Role}", role);
                return false;
            }
            
            // Create the request payload
            var requestData = new { roles = new[] { roleId } };
            var content = new StringContent(
                JsonConvert.SerializeObject(requestData),
                Encoding.UTF8,
                "application/json");
            
            // Add the token to request headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            
            // Send the request
            var response = await _httpClient.PostAsync(apiUrl, content);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully assigned role {Role} to user {UserId}", role, userId);
                return true;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "Failed to assign role. Status: {StatusCode}, Response: {Response}", 
                response.StatusCode, 
                errorContent);
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", role, userId);
            return false;
        }
    }
    
    private async Task<string?> GetRoleIdAsync(string roleName, string token)
    {
        try
        {
            var apiUrl = $"https://{_auth0Settings.Domain}/api/v2/roles";
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            
            var response = await _httpClient.GetAsync(apiUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var roles = JsonConvert.DeserializeObject<List<Auth0Role>>(content);
                
                return roles?.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase))?.Id;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "Failed to retrieve roles. Status: {StatusCode}, Response: {Response}", 
                response.StatusCode, 
                errorContent);
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role ID for {RoleName}", roleName);
            return null;
        }
    }
    
    private class Auth0Role
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}
