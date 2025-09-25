using Microsoft.Extensions.Options;
using SkillBridge.Infrastructure.Configuration;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using System.Text;
using System.Text.Json;

namespace SkillBridge.Infrastructure.Ai
{
    public class N8NClient : IAgentClient
    {
        private readonly HttpClient _httpClient;
        private readonly N8NSettings _settings;
        private readonly JsonSerializerOptions _jsonOptions;

        public N8NClient(HttpClient httpClient, IOptions<N8NSettings> settings)
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
        /// Generates assignment using n8n webhook
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

                // Send directly to n8n webhook
                using var request = new HttpRequestMessage(HttpMethod.Post, _settings.Endpoint)
                {
                    Content = content
                };

                using var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    throw new InvalidOperationException("n8n returned an empty response");
                }

                var assignmentResponse = JsonSerializer.Deserialize<ProjectAssignmentResponse>(responseContent, _jsonOptions);

                if (assignmentResponse == null)
                {
                    throw new InvalidOperationException("Failed to deserialize n8n response to ProjectAssignmentResponse");
                }

                return assignmentResponse;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to communicate with n8n: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to process n8n response: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new InvalidOperationException($"n8n request timed out: {ex.Message}", ex);
            }
        }
    }
}
