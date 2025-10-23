using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
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
using SkillBridge.Services.CurrentUser;
using SkillBridge.Services.File;
using SkillBridge.Services.UserProfile;
using Xunit;

namespace SkillBridge.UnitTests.Services;

public class UserProfileServiceTests
{
    private readonly Mock<AppDbContext> _mockDbContext;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserProfileService>> _mockLogger;
    private readonly Mock<IFileUploader> _mockFileUploader;
    private readonly UserProfileService _userProfileService;

    public UserProfileServiceTests()
    {
        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _mockCurrentUser = new Mock<ICurrentUser>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserProfileService>>();
        _mockFileUploader = new Mock<IFileUploader>();

        _userProfileService = new UserProfileService(
            _mockDbContext.Object,
            _mockCurrentUser.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockFileUploader.Object);
    }

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithUserIdProvided_ReturnsUserProfileResponse()
    {
        // Arrange
        var userId = "auth0|test-user-id";
        var userProfileEntity = CreateUserProfileEntity(userId);
        var expectedResponse = CreateUserProfileResponse(userId);

        var userProfiles = new List<Models.Entities.UserProfile> { userProfileEntity };
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);

        // Act
        var result = await _userProfileService.GetAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.GitHubConnection, result.GitHubConnection);

        _mockMapper.Verify(x => x.Map<UserProfileResponse>(userProfileEntity), Times.Once);
        _mockCurrentUser.Verify(x => x.GetUserId(), Times.Never); // Should not use current user when ID provided
    }

    [Fact]
    public async Task GetAsync_WithoutUserIdProvided_UsesCurrentUser()
    {
        // Arrange
        var currentUserId = "auth0|current-user-id";
        var userProfileEntity = CreateUserProfileEntity(currentUserId);
        var expectedResponse = CreateUserProfileResponse(currentUserId);

        _mockCurrentUser.Setup(x => x.GetUserId()).Returns(currentUserId);

        var userProfiles = new List<Models.Entities.UserProfile> { userProfileEntity };
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);

        // Act
        var result = await _userProfileService.GetAsync(null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);

        _mockCurrentUser.Verify(x => x.GetUserId(), Times.Once);
        _mockMapper.Verify(x => x.Map<UserProfileResponse>(userProfileEntity), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithProfilePictureAndCV_ReturnsFileUrls()
    {
        // Arrange
        var userId = "auth0|test-user-id";
        var userProfileEntity = CreateUserProfileEntity(userId);
        userProfileEntity.ProfilePicture = "profile-pic-path";
        userProfileEntity.CVUpload = "cv-upload-path";

        var expectedResponse = CreateUserProfileResponse(userId);
        var profilePictureUrl = "https://example.com/profile-pic.jpg";
        var cvUrl = "https://example.com/cv.pdf";

        var userProfiles = new List<Models.Entities.UserProfile> { userProfileEntity };
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);
        _mockFileUploader.Setup(x => x.GetFileAsync("profile-pic-path", FileType.Image)).ReturnsAsync(profilePictureUrl);
        _mockFileUploader.Setup(x => x.GetFileAsync("cv-upload-path", FileType.CV)).ReturnsAsync(cvUrl);

        // Act
        var result = await _userProfileService.GetAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(profilePictureUrl, result.ProfilePicture);
        Assert.Equal(cvUrl, result.CVUpload);

        _mockFileUploader.Verify(x => x.GetFileAsync("profile-pic-path", FileType.Image), Times.Once);
        _mockFileUploader.Verify(x => x.GetFileAsync("cv-upload-path", FileType.CV), Times.Once);
    }

    [Fact]
    public async Task GetAsync_UserProfileNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var userId = "auth0|non-existent-user";

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _userProfileService.GetAsync(userId));

        Assert.Equal("Profile", exception.EntityName);
        Assert.Equal($"for user {userId}", exception.EntityId);
        Assert.Contains($"No profile found for user ID {userId}", exception.Message);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedUserProfile()
    {
        // Arrange
        var userId = "auth0|test-user-id";
        var request = CreateValidUpdateUserProfileRequest();
        var userProfileEntity = CreateUserProfileEntity(userId);
        var expectedResponse = CreateUserProfileResponse(userId);

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(userId)).ReturnsAsync(userProfileEntity);
        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);

        // Act
        var result = await _userProfileService.UpdateAsync(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(request.GitHubConnection, userProfileEntity.GitHubConnection);

        mockUserProfileDbSet.Verify(x => x.FindAsync(userId), Times.Once);
        mockUserProfileDbSet.Verify(x => x.Update(userProfileEntity), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        _mockMapper.Verify(x => x.Map<UserProfileResponse>(userProfileEntity), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithoutUserIdProvided_UsesCurrentUser()
    {
        // Arrange
        var currentUserId = "auth0|current-user-id";
        var request = CreateValidUpdateUserProfileRequest();
        var userProfileEntity = CreateUserProfileEntity(currentUserId);
        var expectedResponse = CreateUserProfileResponse(currentUserId);

        _mockCurrentUser.Setup(x => x.GetUserId()).Returns(currentUserId);

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(currentUserId)).ReturnsAsync(userProfileEntity);
        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);

        // Act
        var result = await _userProfileService.UpdateAsync(request, null);

        // Assert
        Assert.NotNull(result);
        _mockCurrentUser.Verify(x => x.GetUserId(), Times.Once);
        mockUserProfileDbSet.Verify(x => x.FindAsync(currentUserId), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UserProfileNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var userId = "auth0|non-existent-user";
        var request = CreateValidUpdateUserProfileRequest();

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(userId)).ReturnsAsync((Models.Entities.UserProfile?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _userProfileService.UpdateAsync(request, userId));

        Assert.Equal("UserProfile", exception.EntityName);
        Assert.Equal(userId, exception.EntityId);
    }

    #endregion

    #region UpdateCVUpload Tests

    [Fact]
    public async Task UpdateCVUpload_WithNewFile_UpdatesCVAndDeletesOld()
    {
        // Arrange
        var userId = "auth0|test-user-id";
        var request = CreateValidCVUploadRequest();
        var userProfileEntity = CreateUserProfileEntity(userId);
        userProfileEntity.CVUpload = "old-cv-path";
        var expectedResponse = CreateUserProfileResponse(userId);
        var newCvPath = "new-cv-path";

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(userId)).ReturnsAsync(userProfileEntity);
        _mockFileUploader.Setup(x => x.UploadFileAsync(request.CVUpload!, FileType.CV)).ReturnsAsync(newCvPath);
        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);

        // Act
        var result = await _userProfileService.UpdateCVUpload(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newCvPath, userProfileEntity.CVUpload);

        _mockFileUploader.Verify(x => x.DeleteFileAsync("old-cv-path", FileType.CV), Times.Once);
        _mockFileUploader.Verify(x => x.UploadFileAsync(request.CVUpload!, FileType.CV), Times.Once);
        mockUserProfileDbSet.Verify(x => x.Update(userProfileEntity), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateCVUpload_WithNullFile_RemovesExistingCV()
    {
        // Arrange
        var userId = "auth0|test-user-id";
        var request = new CVUploadRequest { CVUpload = null };
        var userProfileEntity = CreateUserProfileEntity(userId);
        userProfileEntity.CVUpload = "existing-cv-path";
        var expectedResponse = CreateUserProfileResponse(userId);

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(userId)).ReturnsAsync(userProfileEntity);
        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);

        // Act
        var result = await _userProfileService.UpdateCVUpload(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(userProfileEntity.CVUpload);

        _mockFileUploader.Verify(x => x.DeleteFileAsync("existing-cv-path", FileType.CV), Times.Once);
        _mockFileUploader.Verify(x => x.UploadFileAsync(It.IsAny<IFormFile>(), FileType.CV), Times.Never);
        mockUserProfileDbSet.Verify(x => x.Update(userProfileEntity), Times.Once);
    }

    [Fact]
    public async Task UpdateCVUpload_UserProfileNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var userId = "auth0|non-existent-user";
        var request = CreateValidCVUploadRequest();

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(userId)).ReturnsAsync((Models.Entities.UserProfile?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _userProfileService.UpdateCVUpload(request, userId));

        Assert.Equal("UserProfile", exception.EntityName);
        Assert.Equal(userId, exception.EntityId);
    }

    #endregion

    #region UpdateProfilePicture Tests

    [Fact]
    public async Task UpdateProfilePicture_WithNewFile_UpdatesPictureAndDeletesOld()
    {
        // Arrange
        var userId = "auth0|test-user-id";
        var request = CreateValidProfilePictureRequest();
        var userProfileEntity = CreateUserProfileEntity(userId);
        userProfileEntity.ProfilePicture = "old-picture-path";
        var expectedResponse = CreateUserProfileResponse(userId);
        var newPicturePath = "new-picture-path";

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(userId)).ReturnsAsync(userProfileEntity);
        _mockFileUploader.Setup(x => x.UploadFileAsync(request.ProfilePicture!, FileType.Image)).ReturnsAsync(newPicturePath);
        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);

        // Act
        var result = await _userProfileService.UpdateProfilePicture(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newPicturePath, userProfileEntity.ProfilePicture);

        _mockFileUploader.Verify(x => x.DeleteFileAsync("old-picture-path", FileType.Image), Times.Once);
        _mockFileUploader.Verify(x => x.UploadFileAsync(request.ProfilePicture!, FileType.Image), Times.Once);
        mockUserProfileDbSet.Verify(x => x.Update(userProfileEntity), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateProfilePicture_WithNullFile_RemovesExistingPicture()
    {
        // Arrange
        var userId = "auth0|test-user-id";
        var request = new ProfilePictureRequest { ProfilePicture = null };
        var userProfileEntity = CreateUserProfileEntity(userId);
        userProfileEntity.ProfilePicture = "existing-picture-path";
        var expectedResponse = CreateUserProfileResponse(userId);

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(userId)).ReturnsAsync(userProfileEntity);
        _mockMapper.Setup(x => x.Map<UserProfileResponse>(userProfileEntity)).Returns(expectedResponse);

        // Act
        var result = await _userProfileService.UpdateProfilePicture(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(userProfileEntity.ProfilePicture);

        _mockFileUploader.Verify(x => x.DeleteFileAsync("existing-picture-path", FileType.Image), Times.Once);
        _mockFileUploader.Verify(x => x.UploadFileAsync(It.IsAny<IFormFile>(), FileType.Image), Times.Never);
        mockUserProfileDbSet.Verify(x => x.Update(userProfileEntity), Times.Once);
    }

    [Fact]
    public async Task UpdateProfilePicture_UserProfileNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var userId = "auth0|non-existent-user";
        var request = CreateValidProfilePictureRequest();

        var userProfiles = new List<Models.Entities.UserProfile>();
        var mockUserProfileDbSet = userProfiles.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.UserProfiles).Returns(mockUserProfileDbSet.Object);

        mockUserProfileDbSet.Setup(x => x.FindAsync(userId)).ReturnsAsync((Models.Entities.UserProfile?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _userProfileService.UpdateProfilePicture(request, userId));

        Assert.Equal("UserProfile", exception.EntityName);
        Assert.Equal(userId, exception.EntityId);
    }

    #endregion

    #region Private Helper Methods

    private UpdateUserProfileRequest CreateValidUpdateUserProfileRequest()
    {
        return new UpdateUserProfileRequest
        {
            GitHubConnection = "https://github.com/testuser"
        };
    }

    private CVUploadRequest CreateValidCVUploadRequest()
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test-cv.pdf");
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.ContentType).Returns("application/pdf");
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        return new CVUploadRequest
        {
            CVUpload = mockFile.Object
        };
    }

    private ProfilePictureRequest CreateValidProfilePictureRequest()
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test-profile.jpg");
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        return new ProfilePictureRequest
        {
            ProfilePicture = mockFile.Object
        };
    }

    private Models.Entities.UserProfile CreateUserProfileEntity(string userId)
    {
        return new Models.Entities.UserProfile
        {
            Id = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            ProfilePicture = null,
            CVUpload = null,
            GitHubConnection = "https://github.com/testuser",
            UserProjectAssignments = new List<UserProjectAssignment>()
        };
    }

    private UserProfileResponse CreateUserProfileResponse(string userId)
    {
        return new UserProfileResponse
        {
            Id = userId,
            ProfilePicture = null,
            CVUpload = null,
            GitHubConnection = "https://github.com/testuser"
        };
    }

    #endregion
}