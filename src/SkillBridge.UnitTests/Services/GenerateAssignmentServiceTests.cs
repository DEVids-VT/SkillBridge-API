using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SkillBridge.Infrastructure.Ai;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.GenerateAssignment;
using SkillBridge.Services.ProjectAssignment;
using Xunit;

namespace SkillBridge.UnitTests.Services;

public class GenerateAssignmentServiceTests
{
    private readonly Mock<ILlmClient> _mockLlmClient;
    private readonly Mock<IPromptBuilder> _mockPromptBuilder;
    private readonly Mock<IProjectAssignmentService> _mockProjectAssignmentService;
    private readonly GenerateAssignmentService _generateAssignmentService;

    public GenerateAssignmentServiceTests()
    {
        _mockLlmClient = new Mock<ILlmClient>();
        _mockPromptBuilder = new Mock<IPromptBuilder>();
        _mockProjectAssignmentService = new Mock<IProjectAssignmentService>();

        _generateAssignmentService = new GenerateAssignmentService(
            _mockLlmClient.Object,
            _mockPromptBuilder.Object,
            _mockProjectAssignmentService.Object);
    }

    #region GenerateAssignmentAsync Tests

    [Fact]
    public async Task GenerateAssignmentAsync_ValidRequest_ReturnsProjectAssignmentResponse()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var candidate = CreateCandidateRequirementsRequest();
        
        var generatedAssignment = CreateProjectAssignment();
        
        var assignmentPrompt = new Prompt<ProjectAssignment>("system prompt", "content");
        
        var expectedResponse = new ProjectAssignmentResponse
        {
            Id = Guid.NewGuid(),
            Title = "Test Assignment",
            Description = "Generated description"
        };

        // Setup mocks
        _mockPromptBuilder.Setup(x => x.BuildFromFile<ProjectAssignment>("AssignmentGenerationPrompt.md", candidate))
            .Returns(assignmentPrompt);
        
        _mockLlmClient.Setup(x => x.GenerateAsync(assignmentPrompt)).ReturnsAsync(generatedAssignment);
        
        _mockProjectAssignmentService.Setup(x => x.CreateAsync(companyId, It.IsAny<CreateProjectAssignmentRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _generateAssignmentService.GenerateAssignmentAsync(companyId, candidate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Title, result.Title);

        _mockLlmClient.Verify(x => x.GenerateAsync(assignmentPrompt), Times.Once);
        _mockProjectAssignmentService.Verify(x => x.CreateAsync(companyId, It.IsAny<CreateProjectAssignmentRequest>()), Times.Once);
        
        // Verify that the CreateProjectAssignmentRequest is properly constructed
        _mockProjectAssignmentService.Verify(x => x.CreateAsync(companyId, It.Is<CreateProjectAssignmentRequest>(req =>
            req.Title == generatedAssignment.Title &&
            req.Summary == generatedAssignment.Summary &&
            req.LearningBenefits == generatedAssignment.LearningBenefits &&
            req.SuggestedApproach == generatedAssignment.SuggestedApproach &&
            req.Level == generatedAssignment.Level &&
            req.Status == ProjectAssignmentStatus.Draft &&
            req.Skills.Count == 2 &&
            req.Skills.Contains("C#") &&
            req.Skills.Contains("JavaScript") &&
            req.Tasks.Count == 2
        )), Times.Once);
    }

    [Fact]
    public async Task GenerateAssignmentAsync_LlmReturnsNull_ThrowsException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var candidate = CreateCandidateRequirementsRequest();
        var assignmentPrompt = new Prompt<ProjectAssignment>("system prompt", "content");

        _mockPromptBuilder.Setup(x => x.BuildFromFile<ProjectAssignment>("AssignmentGenerationPrompt.md", candidate))
            .Returns(assignmentPrompt);
        _mockLlmClient.Setup(x => x.GenerateAsync(assignmentPrompt)).ReturnsAsync((ProjectAssignment?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _generateAssignmentService.GenerateAssignmentAsync(companyId, candidate));

        Assert.Equal("Failed to generate assignment from AI.", exception.Message);
    }

    [Fact]
    public async Task GenerateAssignmentAsync_WithTasks_CreatesTasksInCorrectSequence()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var candidate = CreateCandidateRequirementsRequest();
        
        var generatedAssignment = CreateProjectAssignment();
        var assignmentPrompt = new Prompt<ProjectAssignment>("system prompt", "content");
        
        var expectedResponse = new ProjectAssignmentResponse { Id = Guid.NewGuid() };

        // Setup mocks
        _mockPromptBuilder.Setup(x => x.BuildFromFile<ProjectAssignment>("AssignmentGenerationPrompt.md", candidate))
            .Returns(assignmentPrompt);
        
        _mockLlmClient.Setup(x => x.GenerateAsync(assignmentPrompt)).ReturnsAsync(generatedAssignment);
        
        _mockProjectAssignmentService.Setup(x => x.CreateAsync(companyId, It.IsAny<CreateProjectAssignmentRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _generateAssignmentService.GenerateAssignmentAsync(companyId, candidate);

        // Assert
        Assert.NotNull(result);
        
        // Verify that tasks are created with correct sequence
        _mockProjectAssignmentService.Verify(x => x.CreateAsync(companyId, It.Is<CreateProjectAssignmentRequest>(req =>
            req.Tasks.Count == 2 &&
            req.Tasks[0].Title == "Task 1" &&
            req.Tasks[0].Sequence == 1 &&
            req.Tasks[0].IsCompleted == false &&
            req.Tasks[1].Title == "Task 2" &&
            req.Tasks[1].Sequence == 2 &&
            req.Tasks[1].IsCompleted == false
        )), Times.Once);
    }

    [Fact]
    public async Task GenerateAssignmentAsync_WithoutTasks_CreatesEmptyTasksList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var candidate = CreateCandidateRequirementsRequest();
        
        var generatedAssignment = CreateProjectAssignment();
        generatedAssignment.Tasks = new List<AssignmentTask>(); // Empty tasks
        
        var assignmentPrompt = new Prompt<ProjectAssignment>("system prompt", "content");
        var expectedResponse = new ProjectAssignmentResponse { Id = Guid.NewGuid() };

        // Setup mocks
        _mockPromptBuilder.Setup(x => x.BuildFromFile<ProjectAssignment>("AssignmentGenerationPrompt.md", candidate))
            .Returns(assignmentPrompt);
        
        _mockLlmClient.Setup(x => x.GenerateAsync(assignmentPrompt)).ReturnsAsync(generatedAssignment);
        
        _mockProjectAssignmentService.Setup(x => x.CreateAsync(companyId, It.IsAny<CreateProjectAssignmentRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _generateAssignmentService.GenerateAssignmentAsync(companyId, candidate);

        // Assert
        Assert.NotNull(result);
        
        // Verify that empty tasks list is handled correctly
        _mockProjectAssignmentService.Verify(x => x.CreateAsync(companyId, It.Is<CreateProjectAssignmentRequest>(req =>
            req.Tasks.Count == 0
        )), Times.Once);
    }

    #endregion

    #region Private Helper Methods

    private CandidateRequirementsRequest CreateCandidateRequirementsRequest()
    {
        return new CandidateRequirementsRequest
        {
            PositionTitle = "Software Developer",
            DepartmentOrArea = "Engineering",
            CompanyIndustry = "Technology",
            ExperienceLevel = ExperienceLevel.MidLevel,
            MinExperienceYears = 3,
            RequiredCompetencies = new List<CompetencyRequirement>
            {
                new() { Name = "C#", Description = "Programming language", Type = CompetencyType.Technical, RequiredLevel = ProficiencyLevel.Advanced },
                new() { Name = "JavaScript", Description = "Programming language", Type = CompetencyType.Technical, RequiredLevel = ProficiencyLevel.Intermediate }
            },
            PositionSummary = "Software developer position",
            IdealCandidateProfile = "Experienced developer",
            KeyResponsibilities = new List<string> { "Develop software", "Maintain code" },
            CultureFitDescription = "Team player"
        };
    }

    private ProjectAssignment CreateProjectAssignment()
    {
        return new ProjectAssignment
        {
            Id = Guid.NewGuid(),
            Title = "Test Assignment",
            Summary = "Test summary",
            LearningBenefits = "Learning benefits",
            SuggestedApproach = "Suggested approach",
            Level = ProjectAssignmentLevel.Intermediate,
            Tasks = new List<AssignmentTask>
            {
                new AssignmentTask { Title = "Task 1", Description = "Description 1" },
                new AssignmentTask { Title = "Task 2", Description = "Description 2" }
            }
        };
    }

    #endregion
}