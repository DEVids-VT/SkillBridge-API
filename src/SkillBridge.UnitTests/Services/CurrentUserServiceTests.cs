using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Services.CurrentUser;

namespace SkillBridge.UnitTests.Services;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<ClaimsPrincipal> _mockClaimsPrincipal;
    private readonly CurrentUserService _currentUserService;

    public CurrentUserServiceTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
        
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);
        _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
        
        _currentUserService = new CurrentUserService(_mockHttpContextAccessor.Object);
    }

    #region GetUserId Tests

    [Fact]
    public void GetUserId_ValidClaim_ReturnsUserId()
    {
        // Arrange
        var expectedUserId = "auth0|123456789";
        var claim = new Claim("sub", expectedUserId);
        
        _mockClaimsPrincipal.Setup(x => x.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            .Returns((Claim?)null);
        _mockClaimsPrincipal.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
            .Returns((Claim?)null);
        _mockClaimsPrincipal.Setup(x => x.FindFirst("sub"))
            .Returns(claim);

        // Act
        var result = _currentUserService.GetUserId();

        // Assert
        Assert.Equal(expectedUserId, result);
        _mockClaimsPrincipal.Verify(x => x.FindFirst("sub"), Times.Once);
    }

    [Fact]
    public void GetUserId_NoValidClaims_ThrowsAuthenticationException()
    {
        // Arrange
        _mockClaimsPrincipal.Setup(x => x.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            .Returns((Claim?)null);
        _mockClaimsPrincipal.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
            .Returns((Claim?)null);
        _mockClaimsPrincipal.Setup(x => x.FindFirst("sub"))
            .Returns((Claim?)null);

        // Act & Assert
        var exception = Assert.Throws<AuthenticationException>(() => _currentUserService.GetUserId());
        
        Assert.Equal("User is not authenticated or user ID claim is missing", exception.Message);
    }

    #endregion

    #region IsInRole Tests

    [Fact]
    public void IsInRole_UserHasRole_ReturnsTrue()
    {
        // Arrange
        var roleName = "Company";
        _mockClaimsPrincipal.Setup(x => x.IsInRole(roleName)).Returns(true);

        // Act
        var result = _currentUserService.IsInRole(roleName);

        // Assert
        Assert.True(result);
        _mockClaimsPrincipal.Verify(x => x.IsInRole(roleName), Times.Once);
    }

    [Fact]
    public void IsInRole_NullHttpContext_ReturnsFalse()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var result = _currentUserService.IsInRole("SomeRole");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetClaims Tests

    [Fact]
    public void GetClaims_UserHasClaims_ReturnsAllClaims()
    {
        // Arrange
        var expectedClaims = new List<Claim>
        {
            new("sub", "auth0|123456789"),
            new(ClaimTypes.Name, "John Doe"),
            new(ClaimTypes.Role, "Company")
        };

        _mockClaimsPrincipal.Setup(x => x.Claims).Returns(expectedClaims);

        // Act
        var result = _currentUserService.GetClaims();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedClaims.Count, result.Count());
        _mockClaimsPrincipal.Verify(x => x.Claims, Times.Once);
    }

    [Fact]
    public void GetClaims_NullHttpContext_ReturnsEmptyCollection()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var result = _currentUserService.GetClaims();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
}