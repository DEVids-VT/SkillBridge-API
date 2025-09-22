using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkillBridge.Infrastructure.Ai;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.ProjectAssignment;
using SkillBridge.Services.Skill;

namespace SkillBridge.Services.GenerateAssignment
{
    public class GenerateAssignmentService : IGenerateAssignmentService
    {
        private readonly ILlmClient _llmClient;
        private readonly IPromptBuilder _promptBuilder;
        private readonly IProjectAssignmentService _projectAssignmentService;
        private readonly ISkillService _skillService;

        public GenerateAssignmentService(
            ILlmClient llmClient,
            IPromptBuilder promptBuilder,
            IProjectAssignmentService projectAssignmentService,
            ISkillService skillService)
        {
            _llmClient = llmClient ?? throw new ArgumentNullException(nameof(llmClient));
            _promptBuilder = promptBuilder ?? throw new ArgumentNullException(nameof(promptBuilder));
            _projectAssignmentService = projectAssignmentService ?? throw new ArgumentNullException(nameof(projectAssignmentService));
            _skillService = skillService ?? throw new ArgumentNullException(nameof(skillService));
        }

        /// <summary>
        /// Generates and saves a project assignment for a company
        /// </summary>
        /// <param name="companyId">The ID of the company creating the assignment</param>
        /// <param name="candidate">The candidate requirements</param>
        /// <returns>The saved project assignment response</returns>
        public async Task<ProjectAssignmentResponse> GenerateAssignmentAsync(Guid companyId, CandidateRequirementsRequest candidate)
        {
            var prompt = _promptBuilder.BuildFromFile<Models.Entities.ProjectAssignment>("AssignmentGenerationPrompt.md", candidate);
            
            // Generate assignment using LLM client
            var result = await _llmClient.GenerateAsync(prompt);
            
            if (result == null)
                throw new Exception("Failed to generate assignment from AI.");
            
            // Get all skills to find matching ones by name
            var allSkills = await _skillService.GetAllAsync();
            var skillIdsByName = allSkills.ToDictionary(
                s => s.Name.ToLowerInvariant(), 
                s => s.Id);
            
            // Match skill names from the request with existing skill IDs
            // Create skills that don't exist
            var matchingSkillIds = new List<Guid>();
            foreach (var skillName in candidate.RequiredCompetencies)
            {
                if (skillIdsByName.TryGetValue(skillName.Name.ToLowerInvariant(), out var skillId))
                {
                    matchingSkillIds.Add(skillId);
                }
                else
                {
                    // Create the skill if it doesn't exist
                    var createSkillRequest = new CreateSkillRequest { Name = skillName.Name, Description = skillName.Description };
                    var newSkill = await _skillService.CreateAsync(createSkillRequest);
                    matchingSkillIds.Add(newSkill.Id);
                }
            }
            
            // Create tasks from AI result
            var tasks = new List<CreateAssignmentTaskRequest>();
            if (result.Tasks != null && result.Tasks.Any())
            {
                int sequence = 1;
                foreach (var task in result.Tasks)
                {
                    tasks.Add(new CreateAssignmentTaskRequest
                    {
                        Title = task.Title,
                        Description = task.Description,
                        IsCompleted = false, // Tasks start as incomplete
                        Sequence = sequence++
                    });
                }
            }
            
            // Create request for saving the assignment with tasks
            var createRequest = new CreateProjectAssignmentRequest
            {
                Title = result.Title,
                Description = result.Description,
                Summary = result.Summary,
                LearningBenefits = result.LearningBenefits,
                SuggestedApproach = result.SuggestedApproach,
                Level = result.Level,
                Deadline = DateTime.UtcNow.AddDays(14), // Default deadline if not provided by AI
                Status = ProjectAssignmentStatus.Draft,
                SkillIds = matchingSkillIds,
                Tasks = tasks
            };
            
            // Save the generated assignment with tasks to database in a single transaction
            return await _projectAssignmentService.CreateAsync(companyId, createRequest);
        }
    }
}
