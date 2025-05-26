using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SkillBridge.Infrastructure.Configuration;
using SkillBridge.Services;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkillBridge.Tests.Services;

/// <summary>
/// Tests for the <see cref="UserRoleService"/> class
/// </summary>
public class UserRoleServiceTests
{
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<UserRoleService>> _loggerMock;
    private readonly Mock<IOptions<Auth0Settings>> _optionsMock;
    private readonly UserRoleService _userRoleService;
    
    private const string TestUserId = "auth0|12345678";
    private const string TestToken = "test-token";
    private const string TestDomain = "test-domain.auth0.com";
    
    public UserRoleServiceTests()
    {
        _currentUserMock = new Mock<ICurrentUser>();
        _currentUserMock.Setup(u => u.GetUserId()).Returns(TestUserId);
        
        _tokenProviderMock = new Mock<ITokenProvider>();
        _tokenProviderMock.Setup(t => t.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestToken);
        
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        
        _loggerMock = new Mock<ILogger<UserRoleService>>();
        
        var authSettings = new Auth0Settings
        {
            Domain = TestDomain,
            Audience = "test-audience",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };
        
        _optionsMock = new Mock<IOptions<Auth0Settings>>();
        _optionsMock.Setup(o => o.Value).Returns(authSettings);
        
        _userRoleService = new UserRoleService(
            _currentUserMock.Object,
            _tokenProviderMock.Object,
            _httpClient,
            _optionsMock.Object,
            _loggerMock.Object
        );
    }
    
    [Fact]
    public async Task BecomeCompanyAsync_Success_ReturnsTrue()
    {
        // Arrange
        SetupMockHttpClientSuccess();
        
        // Act
        var result = await _userRoleService.BecomeCompanyAsync();
        
        // Assert
        Assert.True(result);
        VerifyHttpClientCalled();
        _currentUserMock.Verify(u => u.GetUserId(), Times.Once);
    }
    
    [Fact]
    public async Task BecomeCompanyAsync_WithUserId_ReturnsTrue()
    {
        // Arrange
        const string customUserId = "auth0|custom";
        SetupMockHttpClientSuccess();
        
        // Act
        var result = await _userRoleService.BecomeCompanyAsync(customUserId);
        
        // Assert
        Assert.True(result);
        VerifyHttpClientCalled();
        _currentUserMock.Verify(u => u.GetUserId(), Times.Never);
    }
    
    [Fact]
    public async Task BecomeCandidateAsync_Success_ReturnsTrue()
    {
        // Arrange
        SetupMockHttpClientSuccess();
        
        // Act
        var result = await _userRoleService.BecomeCandidateAsync();
        
        // Assert
        Assert.True(result);
        VerifyHttpClientCalled();
        _currentUserMock.Verify(u => u.GetUserId(), Times.Once);
    }
    
    [Fact]
    public async Task BecomeCandidateAsync_WithUserId_ReturnsTrue()
    {
        // Arrange
        const string customUserId = "auth0|custom";
        SetupMockHttpClientSuccess();
        
        // Act
        var result = await _userRoleService.BecomeCandidateAsync(customUserId);
        
        // Assert
        Assert.True(result);
        VerifyHttpClientCalled();
        _currentUserMock.Verify(u => u.GetUserId(), Times.Never);
    }
    
    [Fact]
    public async Task BecomeCompanyAsync_HttpClientError_ReturnsFalse()
    {
        // Arrange
        SetupMockHttpClientError();
        
        // Act
        var result = await _userRoleService.BecomeCompanyAsync();
        
        // Assert
        Assert.False(result);
        VerifyHttpClientCalled();
    }
    
    [Fact]
    public async Task BecomeCandidateAsync_HttpClientError_ReturnsFalse()
    {
        // Arrange
        SetupMockHttpClientError();
        
        // Act
        var result = await _userRoleService.BecomeCandidateAsync();
        
        // Assert
        Assert.False(result);
        VerifyHttpClientCalled();
    }
    
    private void SetupMockHttpClientSuccess()
    {
        // Setup response for getRoleId
        var rolesResponse = new[]
        {
            new { id = "role-id-1", name = "Company" },
            new { id = "role-id-2", name = "Candidate" }
        };
        
        var rolesResponseJson = JsonConvert.SerializeObject(rolesResponse);
        var rolesResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(rolesResponseJson, Encoding.UTF8, "application/json")
        };
        
        // Setup response for assign role
        var assignRoleResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        
        _httpMessageHandlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(rolesResponseMessage)
            .ReturnsAsync(assignRoleResponseMessage);
    }
    
    private void SetupMockHttpClientError()
    {
        var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error from Auth0", Encoding.UTF8, "application/json")
        };
        
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(errorResponse);
    }
    
    private void VerifyHttpClientCalled()
    {
        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
    }
}
