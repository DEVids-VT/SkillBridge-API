using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SkillBridge.Models.Enums;
using SkillBridge.Services.File;

namespace SkillBridge.UnitTests.Services;

public class SupabaseBucketFileUploaderTests
{
    private readonly Mock<ILogger<SupabaseBucketFileUploader>> _mockLogger;

    public SupabaseBucketFileUploaderTests()
    {
        _mockLogger = new Mock<ILogger<SupabaseBucketFileUploader>>();
    }

    #region UploadFileAsync Tests

    [Fact]
    public async Task UploadFileAsync_NullFile_ThrowsArgumentException()
    {
        // Arrange - Create service with null Supabase client since we're only testing validation
        var fileUploader = new SupabaseBucketFileUploader(null!, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => fileUploader.UploadFileAsync(null!, FileType.Image));

        Assert.Equal("File is empty.", exception.Message);
    }

    [Fact]
    public async Task UploadFileAsync_EmptyFile_ThrowsArgumentException()
    {
        // Arrange
        var fileUploader = new SupabaseBucketFileUploader(null!, _mockLogger.Object);
        var mockFile = CreateMockFile("test.jpg", "image/jpeg", "");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => fileUploader.UploadFileAsync(mockFile.Object, FileType.Image));

        Assert.Equal("File is empty.", exception.Message);
    }

    [Fact]
    public async Task UploadFileAsync_InvalidImageType_ThrowsInvalidOperationException()
    {
        // Arrange
        var fileUploader = new SupabaseBucketFileUploader(null!, _mockLogger.Object);
        var mockFile = CreateMockFile("test.txt", "text/plain", "test content");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => fileUploader.UploadFileAsync(mockFile.Object, FileType.Image));

        Assert.Contains("Invalid image type", exception.Message);
    }

    [Fact]
    public async Task UploadFileAsync_InvalidCvType_ThrowsInvalidOperationException()
    {
        // Arrange
        var fileUploader = new SupabaseBucketFileUploader(null!, _mockLogger.Object);
        var mockFile = CreateMockFile("test.txt", "text/plain", "test content");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => fileUploader.UploadFileAsync(mockFile.Object, FileType.CV));

        Assert.Contains("Invalid CV type", exception.Message);
    }

    [Fact]
    public async Task UploadFileAsync_InvalidImageExtension_ThrowsInvalidOperationException()
    {
        // Arrange
        var fileUploader = new SupabaseBucketFileUploader(null!, _mockLogger.Object);
        var mockFile = CreateMockFile("test.gif", "image/jpeg", "test content");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => fileUploader.UploadFileAsync(mockFile.Object, FileType.Image));

        Assert.Contains("Invalid image type", exception.Message);
    }

    [Fact]
    public async Task UploadFileAsync_InvalidCvExtension_ThrowsInvalidOperationException()
    {
        // Arrange
        var fileUploader = new SupabaseBucketFileUploader(null!, _mockLogger.Object);
        var mockFile = CreateMockFile("test.txt", "application/pdf", "test content");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => fileUploader.UploadFileAsync(mockFile.Object, FileType.CV));

        Assert.Contains("Invalid CV type", exception.Message);
    }

    #endregion

    #region GetFileAsync Tests

    [Fact]
    public async Task GetFileAsync_UnknownFileType_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var fileUploader = new SupabaseBucketFileUploader(null!, _mockLogger.Object);
        var fileName = "test-file.txt";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => fileUploader.GetFileAsync(fileName, (FileType)999));

        Assert.Equal("type", exception.ParamName);
        // The exception is thrown by GetBucketName method, which uses default ArgumentOutOfRangeException message
    }

    #endregion

    #region DeleteFileAsync Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeleteFileAsync_InvalidFileKey_ThrowsArgumentException(string fileKey)
    {
        // Arrange
        var fileUploader = new SupabaseBucketFileUploader(null!, _mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => fileUploader.DeleteFileAsync(fileKey, FileType.Image));

        Assert.Equal("fileKey", exception.ParamName);
        Assert.Contains("File key cannot be null or empty", exception.Message);
    }

    #endregion

    #region Private Helper Methods

    private Mock<IFormFile> CreateMockFile(string fileName, string contentType, string content)
    {
        var mockFile = new Mock<IFormFile>();
        var bytes = Encoding.UTF8.GetBytes(content);

        mockFile.Setup(x => x.FileName).Returns(fileName);
        mockFile.Setup(x => x.ContentType).Returns(contentType);
        mockFile.Setup(x => x.Length).Returns(bytes.Length);
        mockFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mockFile;
    }

    #endregion
}