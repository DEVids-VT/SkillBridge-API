using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.UserProjectAssignment;
using Xunit;

namespace SkillBridge.UnitTests.Services
{
    public class UserProjectAssignmentServiceTests
    {
        private readonly Mock<AppDbContext> _mockDbContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserProjectAssignmentService>> _mockLogger;
        private readonly UserProjectAssignmentService _service;

        public UserProjectAssignmentServiceTests()
        {
            _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserProjectAssignmentService>>();

            _service = new UserProjectAssignmentService(
                _mockDbContext.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        #region GetUserProjectsAsync Tests

        [Fact]
        public async Task GetUserProjectsAsync_UserExists_ReturnsMappedUserProjectAssignments()
        {
            // Arrange
            var userId = "auth0|user-1";

            var projectAssignment = new ProjectAssignment
            {
                Id = Guid.NewGuid(),
                Title = "Test Project"
            };

            var userProjectAssignmentEntity = new UserProjectAssignment
            {
                UserProfileId = userId,
                ProjectAssignmentId = projectAssignment.Id,
                ClaimedAt = DateTime.UtcNow.AddDays(-1),
                IsCompleted = false,
                ProjectAssignment = projectAssignment
            };

            var userProjectAssignments = new List<UserProjectAssignment> { userProjectAssignmentEntity };
            var mockUpaDbSet = userProjectAssignments.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProjectAssignments).Returns(mockUpaDbSet.Object);

            var userProfiles = new List<UserProfile> { new() { Id = userId } };
            var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

            var expectedProjectAssignmentResponse = new ProjectAssignmentResponse
            {
                Id = projectAssignment.Id,
                Title = projectAssignment.Title
            };

            _mockMapper
                .Setup(m => m.Map<ProjectAssignmentResponse>(It.IsAny<ProjectAssignment>()))
                .Returns(expectedProjectAssignmentResponse);

            // Act
            var result = await _service.GetUserProjectsAsync(userId);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal(expectedProjectAssignmentResponse.Id, list[0].ProjectAssignment.Id);
            Assert.Equal(expectedProjectAssignmentResponse.Title, list[0].ProjectAssignment.Title);

            _mockDbContext.Verify(x => x.UserProfiles, Times.Once);
            _mockDbContext.Verify(x => x.UserProjectAssignments, Times.Once);
            _mockMapper.Verify(m => m.Map<ProjectAssignmentResponse>(It.IsAny<ProjectAssignment>()), Times.Exactly(1));
        }

        [Fact]
        public async Task GetUserProjectsAsync_UserDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userId = "auth0|nonexistent-user";

            var emptyUserProfiles = new List<UserProfile>();
            var mockUserProfileDbSet = emptyUserProfiles.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetUserProjectsAsync(userId));
            Assert.Equal("UserProfile", ex.EntityName);
            Assert.Equal(userId, ex.EntityId);

            _mockDbContext.Verify(x => x.UserProfiles, Times.Once);
            _mockDbContext.Verify(x => x.UserProjectAssignments, Times.Never);
        }

        #endregion

        #region CompleteProjectAsync Tests

        [Fact]
        public async Task CompleteProjectAsync_ValidRequest_MarksAsCompletedAndReturnsResponse()
        {
            // Arrange
            var userId = "auth0|user-1";
            var projectId = Guid.NewGuid();

            var projectAssignment = new ProjectAssignment
            {
                Id = projectId,
                Title = "Test Project"
            };

            var userProjectAssignment = new UserProjectAssignment
            {
                UserProfileId = userId,
                ProjectAssignmentId = projectId,
                ClaimedAt = DateTime.UtcNow.AddDays(-2),
                IsCompleted = false,
                ProjectAssignment = projectAssignment
            };

            var upaList = new List<UserProjectAssignment> { userProjectAssignment };
            var mockUpaDbSet = upaList.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProjectAssignments).Returns(mockUpaDbSet.Object);

            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var expectedPaResponse = new ProjectAssignmentResponse
            {
                Id = projectAssignment.Id,
                Title = projectAssignment.Title
            };

            _mockMapper.Setup(m => m.Map<ProjectAssignmentResponse>(projectAssignment)).Returns(expectedPaResponse);

            var request = new CompleteUserProjectAssignmentRequest
            {
                ProjectAssignmentId = projectId,
                SubmissionRepositoryUrl = "https://github.com/test/repo",
                SubmissionNotes = "Here is my submission"
            };

            // Act
            var result = await _service.CompleteProjectAsync(userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.True(userProjectAssignment.IsCompleted);
            Assert.NotNull(userProjectAssignment.CompletedAt);
            Assert.Equal(request.SubmissionRepositoryUrl, userProjectAssignment.SubmissionRepositoryUrl);
            Assert.NotNull(userProjectAssignment.SubmittedAt);
            Assert.Equal(request.SubmissionNotes, userProjectAssignment.SubmissionNotes);

            Assert.Equal(expectedPaResponse.Id, result.ProjectAssignment.Id);
            Assert.Equal(expectedPaResponse.Title, result.ProjectAssignment.Title);
            Assert.Equal(request.SubmissionRepositoryUrl, result.SubmissionRepositoryUrl);
            Assert.Equal(request.SubmissionNotes, result.SubmissionNotes);
            Assert.NotNull(result.SubmittedAt);

            _mockDbContext.Verify(x => x.UserProjectAssignments, Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(m => m.Map<ProjectAssignmentResponse>(It.IsAny<ProjectAssignment>()), Times.Once);
        }

        [Fact]
        public async Task CompleteProjectAsync_AlreadyCompleted_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = "auth0|user-2";
            var projectId = Guid.NewGuid();

            var projectAssignment = new ProjectAssignment
            {
                Id = projectId,
                Title = "Test Project"
            };

            var userProjectAssignment = new UserProjectAssignment
            {
                UserProfileId = userId,
                ProjectAssignmentId = projectId,
                ClaimedAt = DateTime.UtcNow.AddDays(-2),
                IsCompleted = true, // already completed
                CompletedAt = DateTime.UtcNow.AddDays(-1),
                ProjectAssignment = projectAssignment
            };

            var upaList = new List<UserProjectAssignment> { userProjectAssignment };
            var mockUpaDbSet = upaList.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProjectAssignments).Returns(mockUpaDbSet.Object);

            var request = new CompleteUserProjectAssignmentRequest
            {
                ProjectAssignmentId = projectId,
                SubmissionRepositoryUrl = "https://github.com/test/repo",
                SubmissionNotes = "Here is my submission"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteProjectAsync(userId, request));

            _mockDbContext.Verify(x => x.UserProjectAssignments, Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteProjectAsync_NotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userId = "auth0|user-3";
            var projectId = Guid.NewGuid();

            var emptyUpaList = new List<UserProjectAssignment>();
            var mockUpaDbSet = emptyUpaList.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProjectAssignments).Returns(mockUpaDbSet.Object);

            var request = new CompleteUserProjectAssignmentRequest
            {
                ProjectAssignmentId = projectId
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CompleteProjectAsync(userId, request));
            Assert.Equal("UserProjectAssignment", ex.EntityName);
            Assert.Equal($"User: {userId}, Project: {projectId}", ex.EntityId);

            _mockDbContext.Verify(x => x.UserProjectAssignments, Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region ClaimProjectAsync Tests

        [Fact]
        public async Task ClaimProjectAsync_ValidRequest_AddsUserProjectAssignmentAndReturnsResponse()
        {
            // Arrange
            var userId = "auth0|user-claim";
            var projectId = Guid.NewGuid();
            var duration = TimeSpan.FromDays(5);

            var projectAssignment = new ProjectAssignment
            {
                Id = projectId,
                Title = "Claimable Project",
                Duration = duration
            };

            var projectAssignments = new List<ProjectAssignment> { projectAssignment };
            var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

            var userProfiles = new List<UserProfile> { new() { Id = userId } };
            var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

            var upaList = new List<UserProjectAssignment>(); // empty initially
            var mockUpaDbSet = upaList.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProjectAssignments).Returns(mockUpaDbSet.Object);

            UserProjectAssignment? capturedAdded = null;
            mockUpaDbSet.Setup(x => x.AddAsync(It.IsAny<UserProjectAssignment>(), It.IsAny<CancellationToken>()))
                .Callback<UserProjectAssignment, CancellationToken>((upa, ct) =>
                {
                    capturedAdded = upa;
                    // simulate EF adding to set
                    upaList.Add(upa);
                })
                .Returns(new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserProjectAssignment>>());

            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var expectedPaResponse = new ProjectAssignmentResponse
            {
                Id = projectAssignment.Id,
                Title = projectAssignment.Title
            };
            _mockMapper.Setup(m => m.Map<ProjectAssignmentResponse>(projectAssignment)).Returns(expectedPaResponse);

            var request = new ClaimProjectRequest { ProjectAssignmentId = projectId };

            // Act
            var result = await _service.ClaimProjectAsync(userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(capturedAdded);
            Assert.Equal(userId, capturedAdded!.UserProfileId);
            Assert.Equal(projectId, capturedAdded.ProjectAssignmentId);
            Assert.False(capturedAdded.IsCompleted);
            Assert.NotNull(capturedAdded.Deadline);
            Assert.InRange((capturedAdded.Deadline!.Value - capturedAdded.ClaimedAt).TotalDays, duration.TotalDays - 1, duration.TotalDays + 1);

            Assert.Equal(expectedPaResponse.Id, result.ProjectAssignment.Id);
            Assert.Equal(expectedPaResponse.Title, result.ProjectAssignment.Title);

            mockUpaDbSet.Verify(x => x.AddAsync(It.IsAny<UserProjectAssignment>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(x => x.UserProfiles, Times.Once);
            _mockDbContext.Verify(x => x.ProjectAssignments, Times.Once);
        }

        [Fact]
        public async Task ClaimProjectAsync_UserDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userId = "auth0|no-user";
            var projectId = Guid.NewGuid();

            var emptyUserProfiles = new List<UserProfile>();
            var mockUserProfileDbSet = emptyUserProfiles.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

            var request = new ClaimProjectRequest { ProjectAssignmentId = projectId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.ClaimProjectAsync(userId, request));
            Assert.Equal("UserProfile", ex.EntityName);
            Assert.Equal(userId, ex.EntityId);

            _mockDbContext.Verify(x => x.UserProfiles, Times.Once);
            _mockDbContext.Verify(x => x.ProjectAssignments, Times.Never);
            _mockDbContext.Verify(x => x.UserProjectAssignments, Times.Never);
        }

        [Fact]
        public async Task ClaimProjectAsync_ProjectNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userId = "auth0|user-claim-2";
            var projectId = Guid.NewGuid();

            var userProfiles = new List<UserProfile> { new() { Id = userId } };
            var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

            var emptyProjects = new List<ProjectAssignment>();
            var mockProjectAssignmentDbSet = emptyProjects.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

            var request = new ClaimProjectRequest { ProjectAssignmentId = projectId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.ClaimProjectAsync(userId, request));
            Assert.Equal("ProjectAssignment", ex.EntityName);
            Assert.Equal(request.ProjectAssignmentId, ex.EntityId);

            _mockDbContext.Verify(x => x.UserProfiles, Times.Once);
            _mockDbContext.Verify(x => x.ProjectAssignments, Times.Once);
            _mockDbContext.Verify(x => x.UserProjectAssignments, Times.Never);
        }

        [Fact]
        public async Task ClaimProjectAsync_AlreadyClaimed_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = "auth0|user-claimed";
            var projectId = Guid.NewGuid();

            var projectAssignment = new ProjectAssignment
            {
                Id = projectId,
                Title = "Already Claimed Project",
                Duration = TimeSpan.FromDays(1)
            };

            var projectAssignments = new List<ProjectAssignment> { projectAssignment };
            var mockProjectAssignmentDbSet = projectAssignments.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.ProjectAssignments).Returns(mockProjectAssignmentDbSet.Object);

            var userProfiles = new List<UserProfile> { new() { Id = userId } };
            var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

            var existingUpa = new UserProjectAssignment
            {
                UserProfileId = userId,
                ProjectAssignmentId = projectId,
                ClaimedAt = DateTime.UtcNow.AddDays(-3)
            };
            var upaList = new List<UserProjectAssignment> { existingUpa };
            var mockUpaDbSet = upaList.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(x => x.UserProjectAssignments).Returns(mockUpaDbSet.Object);

            var request = new ClaimProjectRequest { ProjectAssignmentId = projectId };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ClaimProjectAsync(userId, request));

            _mockDbContext.Verify(x => x.UserProfiles, Times.Once);
            _mockDbContext.Verify(x => x.ProjectAssignments, Times.Once);
            _mockDbContext.Verify(x => x.UserProjectAssignments, Times.Once);
        }

        #endregion
    }
}
