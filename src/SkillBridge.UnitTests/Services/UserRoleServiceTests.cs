using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.Extensions.Logging;
using Moq;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Services.CurrentUser;
using SkillBridge.Services.UserRole;
using Xunit;

namespace SkillBridge.UnitTests.Services
{
    public class UserRoleServiceTests
    {
        private readonly Mock<ICurrentUser> _mockCurrentUser;
        private readonly Mock<IRolesClient> _mockRolesClient;
        private readonly Mock<IUsersClient> _mockUsersClient;
        private readonly Mock<ManagementApiClient> _mockManagementApiClient;
        private readonly Mock<ILogger<UserRoleService>> _mockLogger;
        private readonly UserRoleService _userRoleService;

        public UserRoleServiceTests()
        {
            _mockCurrentUser = new Mock<ICurrentUser>();
            _mockRolesClient = new Mock<IRolesClient>();
            _mockUsersClient = new Mock<IUsersClient>();

            // Provide constructor args to avoid requiring a parameterless ctor on ManagementApiClient.
            _mockManagementApiClient = new Mock<ManagementApiClient>(string.Empty, string.Empty);

            _mockLogger = new Mock<ILogger<UserRoleService>>();
            _userRoleService = new UserRoleService(
                _mockCurrentUser.Object,
                _mockRolesClient.Object,
                _mockUsersClient.Object,
                _mockLogger.Object);
        }

        // Helper to invoke private async methods on UserRoleService
        private async Task<T> InvokePrivateAsync<T>(string methodName, params object?[] parameters)
        {
            var method = typeof(UserRoleService).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null) throw new InvalidOperationException($"Method '{methodName}' not found.");

            var taskObj = method.Invoke(_userRoleService, parameters);
            if (taskObj is not Task task) throw new InvalidOperationException($"Invocation of '{methodName}' did not return a Task.");

            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
            if (resultProperty == null) return default!;

            return (T)resultProperty.GetValue(task)!;
        }

        [Fact]
        public async Task BecomeCompanyAsync_RoleExists_AssignsRoleAndReturnsTrue()
        {
            // Arrange
            var userId = "auth0|test-user";
            var roleId = "role-company-id";
            _mockCurrentUser.Setup(x => x.GetUserId()).Returns(userId);

            var rolesList = new List<Role>
            {
                new Role { Id = roleId, Name = "Company" }
            };

            // Mock IPagedList<Role> because RolesClient.GetAllAsync returns Task<IPagedList<Role>>
            var mockPagedList = new Mock<IPagedList<Role>>();
            // Setup enumeration behavior so LINQ/FirstOrDefault works
            mockPagedList.As<IEnumerable<Role>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => rolesList.GetEnumerator());
            mockPagedList.As<IEnumerable>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => rolesList.GetEnumerator());

            // Setup GetAllAsync - match the actual overload used in the service (single parameter)
            _mockRolesClient
                .Setup(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockPagedList.Object);

            // Setup AssignRolesAsync - match the actual overload used in the service (userId, AssignRolesRequest)
            _mockUsersClient
                .Setup(x => x.AssignRolesAsync(userId, It.IsAny<AssignRolesRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userRoleService.BecomeCompanyAsync();

            // Assert
            Assert.True(result);
            _mockCurrentUser.Verify(x => x.GetUserId(), Times.Once);
            _mockRolesClient.Verify(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUsersClient.Verify(x => x.AssignRolesAsync(userId, It.IsAny<AssignRolesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BecomeCandidateAsync_RoleDoesNotExist_ThrowsExternalServiceException()
        {
            // Arrange
            var userId = "auth0|test-user";
            _mockCurrentUser.Setup(x => x.GetUserId()).Returns(userId);

            var emptyRoles = new List<Role>();

            var mockEmptyPagedList = new Mock<IPagedList<Role>>();
            mockEmptyPagedList.As<IEnumerable<Role>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => emptyRoles.GetEnumerator());
            mockEmptyPagedList.As<IEnumerable>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => emptyRoles.GetEnumerator());

            _mockRolesClient
                .Setup(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockEmptyPagedList.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ExternalServiceException>(() => _userRoleService.BecomeCandidateAsync());
            Assert.Contains("Auth0", ex.ServiceName ?? "Auth0");
            _mockRolesClient.Verify(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUsersClient.Verify(x => x.AssignRolesAsync(It.IsAny<string>(), It.IsAny<AssignRolesRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // Tests for private methods

        [Fact]
        public async Task GetRoleIdAsync_ReturnsRoleId_WhenRoleExists()
        {
            // Arrange
            var roleId = "r-123";
            var rolesList = new List<Role> { new Role { Id = roleId, Name = "SpecialRole" } };

            var mockPagedList = new Mock<IPagedList<Role>>();
            mockPagedList.As<IEnumerable<Role>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => rolesList.GetEnumerator());
            mockPagedList.As<IEnumerable>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => rolesList.GetEnumerator());

            _mockRolesClient
                .Setup(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockPagedList.Object);

            // Act
            var result = await InvokePrivateAsync<string?>("GetRoleIdAsync", "SpecialRole");

            // Assert
            Assert.Equal(roleId, result);
            _mockRolesClient.Verify(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRoleIdAsync_ThrowsExternalServiceException_WhenClientThrows()
        {
            // Arrange
            _mockRolesClient
                .Setup(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("auth0 error"));

            // Act & Assert
            await Assert.ThrowsAsync<ExternalServiceException>(async () =>
            {
                await InvokePrivateAsync<string?>("GetRoleIdAsync", "DoesNotMatter");
            });

            _mockRolesClient.Verify(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AssignRoleAsync_AssignsRoleAndReturnsTrue_WhenRoleExists()
        {
            // Arrange
            var userId = "auth0|u1";
            var roleId = "r-company";
            var roleName = "Company";

            var rolesList = new List<Role> { new Role { Id = roleId, Name = roleName } };

            var mockPagedList = new Mock<IPagedList<Role>>();
            mockPagedList.As<IEnumerable<Role>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => rolesList.GetEnumerator());
            mockPagedList.As<IEnumerable>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => rolesList.GetEnumerator());

            _mockRolesClient
                .Setup(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockPagedList.Object);

            _mockUsersClient
                .Setup(x => x.AssignRolesAsync(userId, It.IsAny<AssignRolesRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await InvokePrivateAsync<bool>("AssignRoleAsync", userId, roleName);

            // Assert
            Assert.True(result);
            _mockUsersClient.Verify(x =>
                x.AssignRolesAsync(
                    userId,
                    It.Is<AssignRolesRequest>(r => r.Roles != null && r.Roles.Contains(roleId)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AssignRoleAsync_ThrowsExternalServiceException_WhenAssignFails()
        {
            // Arrange
            var userId = "auth0|u2";
            var roleId = "r-fail";
            var roleName = "Candidate";

            var rolesList = new List<Role> { new Role { Id = roleId, Name = roleName } };

            var mockPagedList = new Mock<IPagedList<Role>>();
            mockPagedList.As<IEnumerable<Role>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => rolesList.GetEnumerator());
            mockPagedList.As<IEnumerable>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => rolesList.GetEnumerator());

            _mockRolesClient
                .Setup(x => x.GetAllAsync(It.IsAny<GetRolesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockPagedList.Object);

            _mockUsersClient
                .Setup(x => x.AssignRolesAsync(userId, It.IsAny<AssignRolesRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("assign failed"));

            // Act & Assert
            await Assert.ThrowsAsync<ExternalServiceException>(async () =>
            {
                await InvokePrivateAsync<bool>("AssignRoleAsync", userId, roleName);
            });

            _mockUsersClient.Verify(x =>
                x.AssignRolesAsync(
                    userId,
                    It.IsAny<AssignRolesRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
