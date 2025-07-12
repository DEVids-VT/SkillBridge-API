using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using SkillBridge.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Ai
{
    /// <summary>
    /// OpenAI implementation of the LLM client interface
    /// </summary>
    public class OpenAILlmClient : ILlmClient
    {
        private readonly ChatClient _chatClient;

        /// <summary>
        /// Creates a new instance of the <see cref="OpenAILlmClient"/> class
        /// </summary>
        /// <param name="openAIOptions">The OpenAI settings</param>
        public OpenAILlmClient(IOptions<OpenAISettings> openAIOptions)
        {
            if (openAIOptions == null)
                throw new ArgumentNullException(nameof(openAIOptions));

            var settings = openAIOptions.Value;
            
            if (string.IsNullOrEmpty(settings.ApiKey))
                throw new ArgumentException("OpenAI API key cannot be null or empty", nameof(openAIOptions));
            
            var openAIClient = new OpenAIClient(settings.ApiKey);
            _chatClient = openAIClient.GetChatClient(settings.Model);
        }

        /// <summary>
        /// Generates a response using a prompt model
        /// </summary>
        /// <typeparam name="TResponse">The expected response type</typeparam>
        /// <param name="promptModel">The prompt model containing the system prompt and data</param>
        /// <returns>The generated response</returns>
        public async Task<TResponse> GenerateAsync<TResponse>(Prompt<TResponse> promptModel) where TResponse : class
        {
            if (promptModel == null)
                throw new ArgumentNullException(nameof(promptModel));

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(promptModel.SystemPrompt)
            };
            
            if (!string.IsNullOrEmpty(promptModel.Content))
            {
                messages.Add(new UserChatMessage(promptModel.Content));
            }

            var chatOptions = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
            };

            var response = await _chatClient.CompleteChatAsync(messages, chatOptions);
            
            if (response == null || response.Value == null || response.Value.Content.Count == 0 || 
                string.IsNullOrEmpty(response.Value.Content[0].Text))
                throw new Exception("Failed to generate response from OpenAI");

            var jsonResponse = response.Value.Content[0].Text;
            
            var parsedResponse = JsonConvert.DeserializeObject<TResponse>(jsonResponse);
            
            if (parsedResponse == null)
                throw new Exception($"Failed to deserialize response to type {typeof(TResponse).Name}");
            
            return parsedResponse;
        }
    }
}