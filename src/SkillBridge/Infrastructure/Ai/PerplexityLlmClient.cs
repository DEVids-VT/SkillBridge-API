using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SkillBridge.Infrastructure.Configuration;

namespace SkillBridge.Infrastructure.Ai
{
    public class PerplexityLlmClient : ILlmClient
    {
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly HttpClient _httpClient;

        public PerplexityLlmClient(IOptions<PerplexitySettings> perplexityOptions)
        {
            if (perplexityOptions == null)
                throw new ArgumentNullException(nameof(perplexityOptions));
            var settings = perplexityOptions.Value;

            if (string.IsNullOrEmpty(settings.ApiKey))
                throw new ArgumentException("Perplexity API key cannot be null or empty", nameof(perplexityOptions));

            if (string.IsNullOrEmpty(settings.Endpoint))
                throw new ArgumentException("Perplexity endpoint cannot be null or empty", nameof(perplexityOptions));

            _apiKey = settings.ApiKey;
            _endpoint = settings.Endpoint;
            _httpClient = new HttpClient();
        }

        public async Task<TResponse> GenerateAsync<TResponse>(Prompt<TResponse> promptModel) where TResponse : class
        {
            if (promptModel == null)
                throw new ArgumentNullException(nameof(promptModel));

            var requestBody = new
            {
                model = "sonar",
                messages = new List<object>
                {
                    new { role = "system", content = promptModel.SystemPrompt },
                    new { role = "user", content = promptModel.Content }
                },
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, _endpoint);
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Content = jsonContent;

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Perplexity API returned error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<dynamic>(responseJson);

            if (apiResponse?.choices == null || apiResponse.choices.Count == 0 ||
                apiResponse.choices?.message == null || apiResponse.choices.message.content == null)
            {
                throw new Exception("Invalid response format from Perplexity API");
            }

            string contentJson = apiResponse.choices.message.content.ToString();
            var parsedResponse = JsonConvert.DeserializeObject<TResponse>(contentJson);

            if (parsedResponse == null)
                throw new Exception($"Failed to deserialize response to type {typeof(TResponse).Name}");

            return parsedResponse;
        }
    }
}