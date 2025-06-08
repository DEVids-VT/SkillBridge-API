using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Options;
using OpenAI;
using SkillBridge.Infrastructure.Configuration;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using System.Text.Json;
using Newtonsoft.Json;
using OpenAI.Chat;
using SkillBridge.Infrastructure.Prompts;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SkillBridge.Services.GenerateAssignment
{
    public class GenerateAssignmentService : IGenerateAssignmentService
    {
        private readonly ChatClient _chatClient;

        public GenerateAssignmentService(ChatClient chatClient, IOptions<OpenAISettings> options)
        {
            _chatClient = chatClient;
        }

        public async Task<Models.Entities.ProjectAssignment> GenerateAssignmentAsync(CandidateRequirementsRequest candidate)
        {
            var prompt = AssignmentGenerationPrompts.GenerateAssignmentPrompt(candidate);

            var messages = new List<ChatMessage>
            {
                new UserChatMessage(prompt)
            };

            var chatOpts = new ChatCompletionOptions()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
            };

            var response = await _chatClient.CompleteChatAsync(messages, chatOpts);
            var json = response.Value.Content[0].Text;

            var parsed = JsonConvert.DeserializeObject<ProjectAssignmentResult>(json);

            if (parsed == null)
                throw new Exception("Failed to parse GPT response.");

            return new Models.Entities.ProjectAssignment
            {
                Title = parsed.Title,
                Description = parsed.Description,
                Deadline = DateTime.UtcNow.AddDays(14),
                Status = ProjectAssignmentStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                ProjectSkills = candidate.RequiredSkills
                    .Select(skill => new ProjectSkill { Skill = new Models.Entities.Skill { Name = skill } })
                    .ToList()
            };
        }
        private class ProjectAssignmentResult
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Deadline { get; set; } = "";
            public List<string> Skills { get; set; } = new();
            public List<string> Requirements { get; set; } = new();
            public List<string> BonusTasks { get; set; } = new();
            public List<string> EvaluationCriteria { get; set; } = new();
        }
    }
}
