using Microsoft.Extensions.Options;
using SkillBridge.Infrastructure.Configuration;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using System.Text;
using System.Text.Json;

namespace SkillBridge.Infrastructure.Ai
{
    /// <summary>
    /// Make.com client implementation for generating assignments through Make.com scenarios
    /// </summary>
    public class MakeComClient : IAgentClient
    {
        private readonly HttpClient _httpClient;
        private readonly MakeComSettings _settings;
        private readonly JsonSerializerOptions _jsonOptions;

        public MakeComClient(HttpClient httpClient, IOptions<MakeComSettings> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        /// <summary>
        /// Generates assignment using Make.com scenario automation
        /// </summary>
        /// <param name="companyId">The company ID requesting the assignment</param>
        /// <param name="candidate">The candidate requirements</param>
        /// <returns>The generated assignment response</returns>
        public async Task<ProjectAssignmentResponse> GenerateAssignment(Guid companyId, CandidateRequirementsRequest candidate)
        {
            try
            {
                // Prepare the request payload
                var requestPayload = new
                {
                    companyId = companyId,
                    candidate = candidate
                };

                var jsonContent = JsonSerializer.Serialize(requestPayload, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Create the HTTP request message
                using var request = new HttpRequestMessage(HttpMethod.Post, _settings.Endpoint)
                {
                    Content = content
                };

                // Add the Make.com API key header
                request.Headers.Add("X-Make-ApiKey", _settings.ApiKey);

                // Send the request
                using var response = await _httpClient.SendAsync(request);

                // Ensure success status code
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    throw new InvalidOperationException("Make.com returned an empty response");
                }

                var assignmentResponse = JsonSerializer.Deserialize<ProjectAssignmentResponse>(responseContent, _jsonOptions);
                
                if (assignmentResponse == null)
                {
                    throw new InvalidOperationException("Failed to deserialize Make.com response to ProjectAssignmentResponse");
                }

                return assignmentResponse;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to communicate with Make.com: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to process Make.com response: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new InvalidOperationException($"Make.com request timed out: {ex.Message}", ex);
            }
        }
    }
}
