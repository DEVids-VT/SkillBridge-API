using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.ProjectAssignment;
using SkillBridge.Services.Skill;
using Xunit;

namespace SkillBridge.UnitTests.Services;

public class ProjectAssignmentServiceTests
{
    private readonly Mock<AppDbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<ProjectAssignmentService>> _mockLogger;
    private readonly Mock<ISkillService> _mockSkillService;
    private readonly ProjectAssignmentService _projectAssignmentService;

    public ProjectAssignmentServiceTests()
    {
        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<ProjectAssignmentService>>();
        _mockSkillService = new Mock<ISkillService>();

        _projectAssignmentService = new ProjectAssignmentService(
            _mockDbContext.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockSkillService.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsProjectAssignmentResponse()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var request = CreateValidCreateProjectAssignmentRequest();

        var projectAssignmentEntity = CreateProjectAssignmentEntity();
        var expectedResponse = CreateProjectAssignmentResponse();

        var companies = new List<Company> { new() { Id = companyId } };
        var mockCompanyDbSet = companies.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Companies).Returns(mockCompanyDbSet.Object);

        var validatedSkillIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _mockSkillService.Setup(x => x.GetOrCreateSkillsByNameAsync(request.Skills)).ReturnsAsync(validatedSkillIds);
        _mockMapper.Setup(x => x.Map<ProjectAssignment>(request)).Returns(projectAssignmentEntity);

        var projectAssignments = new List<ProjectAssignment> { projectAssignmentEntity };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        _mockMapper.Setup(x => x.Map<ProjectAssignmentResponse>(It.IsAny<ProjectAssignment>())).Returns(expectedResponse);

        // Act
        var result = await _projectAssignmentService.CreateAsync(companyId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Title, result.Title);

        _mockSkillService.Verify(x => x.GetOrCreateSkillsByNameAsync(request.Skills), Times.Once);
        _mockMapper.Verify(x => x.Map<ProjectAssignment>(request), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CompanyNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var request = CreateValidCreateProjectAssignmentRequest();

        var companies = new List<Company>();
        var mockCompanyDbSet = companies.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Companies).Returns(mockCompanyDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.CreateAsync(companyId, request));

        Assert.Equal("Company", exception.EntityName);
        Assert.Equal(companyId, exception.EntityId);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsProjectAssignmentResponse()
    {
        // Arrange
        var projectAssignmentId = Guid.NewGuid();
        var projectAssignmentEntity = CreateProjectAssignmentEntity();
        projectAssignmentEntity.Id = projectAssignmentId;
        var expectedResponse = CreateProjectAssignmentResponse();

        var projectAssignments = new List<ProjectAssignment> { projectAssignmentEntity };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        _mockMapper.Setup(x => x.Map<ProjectAssignmentResponse>(projectAssignmentEntity)).Returns(expectedResponse);

        // Act
        var result = await _projectAssignmentService.GetByIdAsync(projectAssignmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectAssignmentId = Guid.NewGuid();

        var projectAssignments = new List<ProjectAssignment>();
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.GetByIdAsync(projectAssignmentId));

        Assert.Equal("ProjectAssignment", exception.EntityName);
        Assert.Equal(projectAssignmentId, exception.EntityId);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllProjectAssignments()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var projectAssignments = new List<ProjectAssignment>
        {
            CreateProjectAssignmentEntity(),
            CreateProjectAssignmentEntity(),
            CreateProjectAssignmentEntity()
        };

        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        _mockMapper.Setup(x => x.Map<ProjectAssignmentResponse>(It.IsAny<ProjectAssignment>()))
            .Returns((ProjectAssignment pa) => new ProjectAssignmentResponse { Id = pa.Id, Title = pa.Title });

        // Act
        var result = await _projectAssignmentService.GetAllAsync(pageIndex, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(pageIndex, result.PageIndex);
        Assert.Equal(pageSize, result.PageSize);
    }

    #endregion

    #region GetByCompanyIdAsync Tests

    [Fact]
    public async Task GetByCompanyIdAsync_ValidCompanyId_ReturnsProjectAssignments()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var projectAssignments = new List<ProjectAssignment>
        {
            CreateProjectAssignmentEntity(companyId),
            CreateProjectAssignmentEntity(companyId)
        };

        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        _mockMapper.Setup(x => x.Map<ProjectAssignmentResponse>(It.IsAny<ProjectAssignment>()))
            .Returns((ProjectAssignment pa) => new ProjectAssignmentResponse { Id = pa.Id, Title = pa.Title });

        // Act
        var result = await _projectAssignmentService.GetByCompanyIdAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedProjectAssignment()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = CreateValidUpdateProjectAssignmentRequest();
        var projectAssignmentEntity = CreateProjectAssignmentEntity();
        projectAssignmentEntity.Id = projectId;
        var expectedResponse = CreateProjectAssignmentResponse();

        var projectAssignments = new List<ProjectAssignment> { projectAssignmentEntity };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        var validatedSkillIds = new List<Guid> { Guid.NewGuid() };
        _mockSkillService.Setup(x => x.GetOrCreateSkillsByNameAsync(request.Skills)).ReturnsAsync(validatedSkillIds);
        _mockMapper.Setup(x => x.Map<ProjectAssignmentResponse>(It.IsAny<ProjectAssignment>())).Returns(expectedResponse);

        var projectSkills = new List<ProjectSkill>();
        var mockProjectSkillDbSet = projectSkills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectSkills).Returns(mockProjectSkillDbSet.Object);

        // Act
        var result = await _projectAssignmentService.UpdateAsync(projectId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        _mockSkillService.Verify(x => x.GetOrCreateSkillsByNameAsync(request.Skills), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ProjectNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = CreateValidUpdateProjectAssignmentRequest();

        var projectAssignments = new List<ProjectAssignment>();
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.UpdateAsync(projectId, request));

        Assert.Equal("ProjectAssignment", exception.EntityName);
        Assert.Equal(projectId, exception.EntityId);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ValidId_DeletesProjectAssignment()
    {
        // Arrange
        var projectAssignmentId = Guid.NewGuid();
        var projectAssignmentEntity = CreateProjectAssignmentEntity();
        projectAssignmentEntity.Id = projectAssignmentId;

        var projectAssignments = new List<ProjectAssignment> { projectAssignmentEntity };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        var projectSkills = new List<ProjectSkill>();
        var mockProjectSkillDbSet = projectSkills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectSkills).Returns(mockProjectSkillDbSet.Object);

        // Act
        await _projectAssignmentService.DeleteAsync(projectAssignmentId);

        // Assert
        mockProjectSkillDbSet.Verify(x => x.RemoveRange(It.IsAny<IEnumerable<ProjectSkill>>()), Times.Once);
        mockProjectAssignmentDbSet.Verify(x => x.Remove(projectAssignmentEntity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectAssignmentId = Guid.NewGuid();

        var projectAssignments = new List<ProjectAssignment>();
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.DeleteAsync(projectAssignmentId));

        Assert.Equal("ProjectAssignment", exception.EntityName);
        Assert.Equal(projectAssignmentId, exception.EntityId);
    }

    #endregion

    #region CreateTaskAsync Tests

    [Fact]
    public async Task CreateTaskAsync_ValidRequest_ReturnsAssignmentTaskResponse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = CreateValidCreateAssignmentTaskRequest();
        var taskEntity = CreateAssignmentTaskEntity();
        var expectedResponse = CreateAssignmentTaskResponse();

        var projectAssignments = new List<ProjectAssignment> { new() { Id = projectId } };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        var tasks = new List<AssignmentTask>();
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        _mockMapper.Setup(x => x.Map<AssignmentTask>(request)).Returns(taskEntity);
        _mockMapper.Setup(x => x.Map<AssignmentTaskResponse>(taskEntity)).Returns(expectedResponse);

        // Act
        var result = await _projectAssignmentService.CreateTaskAsync(projectId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Title, result.Title);

        _mockMapper.Verify(x => x.Map<AssignmentTask>(request), Times.Once);
        _mockMapper.Verify(x => x.Map<AssignmentTaskResponse>(taskEntity), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_ProjectNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = CreateValidCreateAssignmentTaskRequest();

        var projectAssignments = new List<ProjectAssignment>();
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.CreateTaskAsync(projectId, request));

        Assert.Equal("ProjectAssignment", exception.EntityName);
        Assert.Equal(projectId, exception.EntityId);
    }

    #endregion

    #region GetTasksAsync Tests

    [Fact]
    public async Task GetTasksAsync_ValidProjectId_ReturnsTaskList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var projectAssignments = new List<ProjectAssignment> { new() { Id = projectId } };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        var tasks = new List<AssignmentTask>
        {
            CreateAssignmentTaskEntity(projectId),
            CreateAssignmentTaskEntity(projectId)
        };
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        _mockMapper.Setup(x => x.Map<AssignmentTaskResponse>(It.IsAny<AssignmentTask>()))
            .Returns((AssignmentTask t) => new AssignmentTaskResponse { Id = t.Id, Title = t.Title });

        // Act
        var result = await _projectAssignmentService.GetTasksAsync(projectId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetTasksAsync_ProjectNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        var projectAssignments = new List<ProjectAssignment>();
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.GetTasksAsync(projectId));

        Assert.Equal("ProjectAssignment", exception.EntityName);
        Assert.Equal(projectId, exception.EntityId);
    }

    #endregion

    #region GetTaskByIdAsync Tests

    [Fact]
    public async Task GetTaskByIdAsync_ValidIds_ReturnsTask()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var taskEntity = CreateAssignmentTaskEntity(projectId, taskId);
        var expectedResponse = CreateAssignmentTaskResponse(taskId);

        var tasks = new List<AssignmentTask> { taskEntity };
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        _mockMapper.Setup(x => x.Map<AssignmentTaskResponse>(taskEntity)).Returns(expectedResponse);

        // Act
        var result = await _projectAssignmentService.GetTaskByIdAsync(projectId, taskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Title, result.Title);
    }

    [Fact]
    public async Task GetTaskByIdAsync_TaskNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var tasks = new List<AssignmentTask>();
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.GetTaskByIdAsync(projectId, taskId));

        Assert.Equal("AssignmentTask", exception.EntityName);
        Assert.Equal(taskId, exception.EntityId);
    }

    #endregion

    #region UpdateTaskAsync Tests

    [Fact]
    public async Task UpdateTaskAsync_ValidRequest_ReturnsUpdatedTask()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var request = CreateValidUpdateAssignmentTaskRequest();
        var taskEntity = CreateAssignmentTaskEntity(projectId, taskId);
        var expectedResponse = CreateAssignmentTaskResponse(taskId);

        var tasks = new List<AssignmentTask> { taskEntity };
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        _mockMapper.Setup(x => x.Map<AssignmentTaskResponse>(taskEntity)).Returns(expectedResponse);

        // Act
        var result = await _projectAssignmentService.UpdateTaskAsync(projectId, taskId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        _mockMapper.Verify(x => x.Map(request, taskEntity), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var request = CreateValidUpdateAssignmentTaskRequest();

        var tasks = new List<AssignmentTask>();
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.UpdateTaskAsync(projectId, taskId, request));

        Assert.Equal("AssignmentTask", exception.EntityName);
        Assert.Equal(taskId, exception.EntityId);
    }

    #endregion

    #region DeleteTaskAsync Tests

    [Fact]
    public async Task DeleteTaskAsync_ValidIds_DeletesTask()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var taskEntity = CreateAssignmentTaskEntity(projectId, taskId);

        var tasks = new List<AssignmentTask> { taskEntity };
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        // Act
        await _projectAssignmentService.DeleteTaskAsync(projectId, taskId);

        // Assert
        mockAssignmentTaskDbSet.Verify(x => x.Remove(taskEntity), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_TaskNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var tasks = new List<AssignmentTask>();
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.DeleteTaskAsync(projectId, taskId));

        Assert.Equal("AssignmentTask", exception.EntityName);
        Assert.Equal(taskId, exception.EntityId);
    }

    #endregion

    #region CompleteTaskAsync Tests

    [Fact]
    public async Task CompleteTaskAsync_TaskNotCompleted_SetsTaskToCompleted()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var taskEntity = CreateAssignmentTaskEntity(projectId, taskId);
        taskEntity.IsCompleted = false;

        var projectAssignmentEntity = CreateProjectAssignmentEntity(Guid.NewGuid(), projectId);
        projectAssignmentEntity.Tasks = new List<AssignmentTask> { taskEntity };

        var projectAssignments = new List<ProjectAssignment> { projectAssignmentEntity };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        var tasks = new List<AssignmentTask> { taskEntity };
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        var expectedResponse = CreateAssignmentTaskResponse(taskId);
        _mockMapper.Setup(x => x.Map<AssignmentTaskResponse>(taskEntity)).Returns(expectedResponse);

        // Act
        var result = await _projectAssignmentService.CompleteTaskAsync(projectId, taskId);

        // Assert
        Assert.NotNull(result);
        Assert.True(taskEntity.IsCompleted);
        mockAssignmentTaskDbSet.Verify(x => x.Update(taskEntity), Times.Once);
    }

    [Fact]
    public async Task CompleteTaskAsync_TaskCompleted_SetsTaskToNotCompleted()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var taskEntity = CreateAssignmentTaskEntity(projectId, taskId);
        taskEntity.IsCompleted = true;

        var projectAssignmentEntity = CreateProjectAssignmentEntity(Guid.NewGuid(), projectId);
        projectAssignmentEntity.Tasks = new List<AssignmentTask> { taskEntity };

        var projectAssignments = new List<ProjectAssignment> { projectAssignmentEntity };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        var tasks = new List<AssignmentTask> { taskEntity };
        var mockAssignmentTaskDbSet = tasks.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.AssignmentTasks).Returns(mockAssignmentTaskDbSet.Object);

        var expectedResponse = CreateAssignmentTaskResponse(taskId);
        _mockMapper.Setup(x => x.Map<AssignmentTaskResponse>(taskEntity)).Returns(expectedResponse);

        // Act
        var result = await _projectAssignmentService.CompleteTaskAsync(projectId, taskId);

        // Assert
        Assert.NotNull(result);
        Assert.False(taskEntity.IsCompleted);
        mockAssignmentTaskDbSet.Verify(x => x.Update(taskEntity), Times.Once);
    }

    [Fact]
    public async Task CompleteTaskAsync_ProjectNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var projectAssignments = new List<ProjectAssignment>();
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.CompleteTaskAsync(projectId, taskId));

        Assert.Equal("ProjectAssignment", exception.EntityName);
        Assert.Equal(projectId, exception.EntityId);
    }

    [Fact]
    public async Task CompleteTaskAsync_TaskNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var projectAssignmentEntity = CreateProjectAssignmentEntity(Guid.NewGuid(), projectId);
        projectAssignmentEntity.Tasks = new List<AssignmentTask>();

        var projectAssignments = new List<ProjectAssignment> { projectAssignmentEntity };
        var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _projectAssignmentService.CompleteTaskAsync(projectId, taskId));

        Assert.Equal("AssignmentTask", exception.EntityName);
        Assert.Equal(taskId, exception.EntityId);
    }

    #endregion

    #region Private Helper Methods

    private CreateProjectAssignmentRequest CreateValidCreateProjectAssignmentRequest()
    {
        return new CreateProjectAssignmentRequest
        {
            Title = "Test Project Assignment",
            Description = "Test Description",
            Summary = "Test Summary",
            LearningBenefits = "Test Learning Benefits",
            SuggestedApproach = "Test Approach",
            Level = ProjectAssignmentLevel.Intermediate,
            Duration = TimeSpan.FromDays(3),
            Status = ProjectAssignmentStatus.Published,
            Skills = new List<string> {"skill1", "skill2" },
            Tasks = new List<CreateAssignmentTaskRequest>
            {
                new() { Title = "Task 1", Description = "Task 1 Description", Sequence = 1 },
                new() { Title = "Task 2", Description = "Task 2 Description", Sequence = 2 }
            }
        };
    }

    private UpdateProjectAssignmentRequest CreateValidUpdateProjectAssignmentRequest()
    {
        return new UpdateProjectAssignmentRequest
        {
            Title = "Updated Project Assignment",
            Description = "Updated Description",
            Summary = "Updated Summary",
            LearningBenefits = "Updated Learning Benefits",
            SuggestedApproach = "Updated Approach",
            Level = ProjectAssignmentLevel.Advanced,
            Duration = TimeSpan.FromDays(3),
            Status = ProjectAssignmentStatus.Published,
            Skills = new List<string> { "skill1", "skillupdated" },
        };
    }

    private ProjectAssignment CreateProjectAssignmentEntity(Guid? companyId = null, Guid? projectId = null)
    {
        return new ProjectAssignment
        {
            Id = projectId ?? Guid.NewGuid(),
            Title = "Test Project Assignment",
            Description = "Test Description",
            Summary = "Test Summary",
            LearningBenefits = "Test Learning Benefits",
            SuggestedApproach = "Test Approach",
            Level = ProjectAssignmentLevel.Intermediate,
            Status = ProjectAssignmentStatus.Published,
            CompanyId = companyId ?? Guid.NewGuid(),
            ProjectSkills = new List<ProjectSkill>(),
            Tasks = new List<AssignmentTask>()
        };
    }

    private ProjectAssignmentResponse CreateProjectAssignmentResponse()
    {
        return new ProjectAssignmentResponse
        {
            Id = Guid.NewGuid(),
            Title = "Test Project Assignment",
            Description = "Test Description",
            Level = ProjectAssignmentLevel.Intermediate,
            Status = ProjectAssignmentStatus.Published
        };
    }

    private CreateAssignmentTaskRequest CreateValidCreateAssignmentTaskRequest()
    {
        return new CreateAssignmentTaskRequest
        {
            Title = "Test Task",
            Description = "Test Task Description",
            IsCompleted = false,
            Sequence = 1
        };
    }

    private UpdateAssignmentTaskRequest CreateValidUpdateAssignmentTaskRequest()
    {
        return new UpdateAssignmentTaskRequest
        {
            Title = "Updated Task",
            Description = "Updated Task Description",
            IsCompleted = true,
            Sequence = 2
        };
    }

    private AssignmentTask CreateAssignmentTaskEntity(Guid? projectId = null, Guid? taskId = null)
    {
        return new AssignmentTask
        {
            Id = taskId ?? Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Task Description",
            IsCompleted = false,
            Sequence = 1,
            ProjectAssignmentId = projectId ?? Guid.NewGuid()
        };
    }

    private AssignmentTaskResponse CreateAssignmentTaskResponse(Guid? taskId = null)
    {
        return new AssignmentTaskResponse
        {
            Id = taskId ?? Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Task Description",
            IsCompleted = false,
            Sequence = 1,
            ProjectAssignmentId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
    }

    #endregion
}