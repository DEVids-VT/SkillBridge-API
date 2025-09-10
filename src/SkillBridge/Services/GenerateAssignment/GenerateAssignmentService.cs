using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SkillBridge.Infrastructure.Ai;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.ProjectAssignment;

namespace SkillBridge.Services.GenerateAssignment
{
    public class GenerateAssignmentService : IGenerateAssignmentService
    {
        private readonly ILlmClient _llmClient;
        private readonly IPromptBuilder _promptBuilder;
        private readonly IProjectAssignmentService _projectAssignmentService;

        public GenerateAssignmentService(
            ILlmClient llmClient,
            IPromptBuilder promptBuilder,
            IProjectAssignmentService projectAssignmentService)
        {
            _llmClient = llmClient ?? throw new ArgumentNullException(nameof(llmClient));
            _promptBuilder = promptBuilder ?? throw new ArgumentNullException(nameof(promptBuilder));
            _projectAssignmentService = projectAssignmentService ?? throw new ArgumentNullException(nameof(projectAssignmentService));
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

           // var descriptionPrompt = _promptBuilder.BuildFromFile<DescriptionModel>("AssignmentDescriptionGenerationPrompt.md", result);
            //var description = await _llmClient.GenerateAsync(descriptionPrompt);

            if (result == null)
                throw new Exception("Failed to generate assignment from AI.");
            
            // Extract skill names from the candidate requirements
            // The CreateProjectAssignmentService will handle creating any missing skills automatically
            var skillNames = candidate.RequiredCompetencies.Select(skill => skill.Name).ToList();
            
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
                Duration = TimeSpan.FromDays(14), // Default deadline if not provided by AI
                Status = ProjectAssignmentStatus.Draft,
                Skills = skillNames,
                Tasks = tasks
            };
            
            // Save the generated assignment with tasks to database in a single transaction
            return await _projectAssignmentService.CreateAsync(companyId, createRequest);
        }
    }

    public class DescriptionModel
    {
        public string Description { get; set; } = default!;
    }
}
