using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Options;
using OpenAI;
using SkillBridge.Infrastructure.Configuration;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using System.Text.Json;
using Newtonsoft.Json;
using OpenAI.Chat;
using SkillBridge.Infrastructure.Prompts;
using SkillBridge.Services.ProjectAssignment;
using SkillBridge.Services.Skill;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SkillBridge.Services.GenerateAssignment
{
    public class GenerateAssignmentService : IGenerateAssignmentService
    {
        private readonly ChatClient _chatClient;
        private readonly IProjectAssignmentService _projectAssignmentService;
        private readonly ISkillService _skillService;

        public GenerateAssignmentService(
            ChatClient chatClient, 
            IOptions<OpenAISettings> options,
            IProjectAssignmentService projectAssignmentService,
            ISkillService skillService)
        {
            _chatClient = chatClient;
            _projectAssignmentService = projectAssignmentService;
            _skillService = skillService;
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

        public async Task<ProjectAssignmentResponse> GenerateAndSaveAssignmentAsync(Guid companyId, CandidateRequirementsRequest candidate)
        {
            // Generate the assignment using AI
            var generatedAssignment = await GenerateAssignmentAsync(candidate);
            
            // Get all skills to find matching ones by name
            var allSkills = await _skillService.GetAllAsync();
            var skillIdsByName = allSkills.ToDictionary(
                s => s.Name.ToLowerInvariant(), 
                s => s.Id);
            
            // Match skill names from the request with existing skill IDs
            // Create skills that don't exist
            var matchingSkillIds = new List<Guid>();
            foreach (var skillName in candidate.RequiredSkills)
            {
                if (skillIdsByName.TryGetValue(skillName.ToLowerInvariant(), out var skillId))
                {
                    matchingSkillIds.Add(skillId);
                }
                else
                {
                    // Create the skill if it doesn't exist
                    var createSkillRequest = new CreateSkillRequest { Name = skillName };
                    var newSkill = await _skillService.CreateAsync(createSkillRequest);
                    matchingSkillIds.Add(newSkill.Id);
                }
            }
            
            var createRequest = new CreateProjectAssignmentRequest
            {
                Title = generatedAssignment.Title,
                Description = generatedAssignment.Description,
                Deadline = generatedAssignment.Deadline,
                Status = generatedAssignment.Status,
                SkillIds = matchingSkillIds
            };
            
            // Save the generated assignment to database through the project assignment service
            return await _projectAssignmentService.CreateAsync(companyId, createRequest);
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
