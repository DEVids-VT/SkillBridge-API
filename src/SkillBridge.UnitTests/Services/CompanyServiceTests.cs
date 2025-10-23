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
using SkillBridge.Services.Company;
using SkillBridge.Services.CurrentUser;
using SkillBridge.Services.File;
using Xunit;

namespace SkillBridge.UnitTests.Services;

public class CompanyServiceTests
{
    private readonly Mock<AppDbContext> _mockDbContext;
    private readonly Mock<DbSet<Company>> _mockCompanyDbSet;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<CompanyService>> _mockLogger;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly Mock<IFileUploader> _mockFileUploader;
    private readonly CompanyService _companyService;

    public CompanyServiceTests()
    {
        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _mockCompanyDbSet = new Mock<DbSet<Company>>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<CompanyService>>();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _mockFileUploader = new Mock<IFileUploader>();

        _mockDbContext.Setup(x => x.Companies).Returns(_mockCompanyDbSet.Object);
        
        _companyService = new CompanyService(
            _mockDbContext.Object, 
            _mockMapper.Object, 
            _mockLogger.Object, 
            _mockCurrentUser.Object,
            _mockFileUploader.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCompanyResponse()
    {
        // Arrange
        var request = new CreateCompanyRequest
        {
            Name = "Test Company",
            About = "Test About",
            Activities = "Test Activities",
            Sector = "Test Sector",
            HeadOfficeLocation = "Test Location",
            Technologies = "Test Technologies",
            EmployeesWorldwide = 100,
            WebsiteUrl = "https://test.com",
            ContactName = "Test Contact",
            ContactEmail = "test@test.com",
            ContactPhone = "123-456-7890"
        };

        var companyEntity = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            About = request.About,
            Auth0UserId = "test-user-id"
        };

        var expectedResponse = new CompanyResponse
        {
            Id = companyEntity.Id,
            Name = companyEntity.Name,
            About = companyEntity.About,
            Auth0UserId = companyEntity.Auth0UserId
        };

        _mockCurrentUser.Setup(x => x.GetUserId()).Returns("test-user-id");
        _mockMapper.Setup(x => x.Map<Company>(request)).Returns(companyEntity);
        _mockMapper.Setup(x => x.Map<CompanyResponse>(It.IsAny<Company>())).Returns(expectedResponse);

        _mockCompanyDbSet.Setup(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Company>>());
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _companyService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
        Assert.Equal("test-user-id", result.Auth0UserId);

        _mockCurrentUser.Verify(x => x.GetUserId(), Times.Once);
        _mockMapper.Verify(x => x.Map<Company>(request), Times.Once);
        _mockMapper.Verify(x => x.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
        _mockCompanyDbSet.Verify(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingCompany_ReturnsCompanyResponse()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company
        {
            Id = companyId,
            Name = "Test Company",
            About = "Test About",
            Auth0UserId = "test-user-id"
        };

        var expectedResponse = new CompanyResponse
        {
            Id = companyId,
            Name = "Test Company",
            About = "Test About"
        };

        _mockCompanyDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>())).ReturnsAsync(company);
        _mockMapper.Setup(x => x.Map<CompanyResponse>(company)).Returns(expectedResponse);

        // Act
        var result = await _companyService.GetByIdAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);

        _mockCompanyDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>()), Times.Once);
        _mockMapper.Verify(x => x.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentCompany_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockCompanyDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>())).ReturnsAsync((Company?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _companyService.GetByIdAsync(nonExistentId));

        Assert.Equal("Company", exception.EntityName);
        Assert.Equal(nonExistentId, exception.EntityId);

        _mockCompanyDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>()), Times.Once);
    }

    #endregion

    #region GetMyCompanyAsync Tests

    [Fact]
    public async Task GetMyCompanyAsync_WithoutUserId_UsesCurrentUser_ReturnsCompanyResponse()
    {
        // Arrange
        var userId = "current-user-id";
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "My Company",
            Auth0UserId = userId
        };

        var expectedResponse = new CompanyResponse
        {
            Id = company.Id,
            Name = "My Company",
            Auth0UserId = userId
        };

        var companies = new List<Company> { company };
        var mockQueryable = companies.AsQueryable().BuildMockDbSet();

        _mockDbContext.Setup(x => x.Companies).Returns(mockQueryable.Object);
        _mockCurrentUser.Setup(x => x.GetUserId()).Returns(userId);
        _mockMapper.Setup(x => x.Map<CompanyResponse>(company)).Returns(expectedResponse);

        // Act
        var result = await _companyService.GetMyCompanyAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Auth0UserId, result.Auth0UserId);

        _mockCurrentUser.Verify(x => x.GetUserId(), Times.Once);
        _mockMapper.Verify(x => x.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task GetMyCompanyAsync_WithUserId_UsesProvidedUserId_ReturnsCompanyResponse()
    {
        // Arrange
        var providedUserId = "provided-user-id";
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "User Company",
            Auth0UserId = providedUserId
        };

        var expectedResponse = new CompanyResponse
        {
            Id = company.Id,
            Name = "User Company",
            Auth0UserId = providedUserId
        };

        var companies = new List<Company> { company };
        var mockQueryable = companies.AsQueryable().BuildMockDbSet();

        _mockDbContext.Setup(x => x.Companies).Returns(mockQueryable.Object);
        _mockMapper.Setup(x => x.Map<CompanyResponse>(company)).Returns(expectedResponse);

        // Act
        var result = await _companyService.GetMyCompanyAsync(providedUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(providedUserId, result.Auth0UserId);

        _mockCurrentUser.Verify(x => x.GetUserId(), Times.Never);
        _mockMapper.Verify(x => x.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task GetMyCompanyAsync_UserHasNoCompany_ThrowsEntityNotFoundException()
    {
        // Arrange
        var userId = "user-without-company";
        var companies = new List<Company>(); // Empty list
        var mockQueryable = companies.AsQueryable().BuildMockDbSet();

        _mockDbContext.Setup(x => x.Companies).Returns(mockQueryable.Object);
        _mockCurrentUser.Setup(x => x.GetUserId()).Returns(userId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _companyService.GetMyCompanyAsync());

        Assert.Equal("Company", exception.EntityName);
        Assert.Equal($"for user {userId}", exception.EntityId);
        Assert.Contains($"No company found for user ID {userId}", exception.Message);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_NoCompanies_ReturnsEmptyCollection()
    {
        // Arrange
        var companies = new List<Company>();
        var mockQueryable = companies.AsQueryable().BuildMockDbSet();

        _mockDbContext.Setup(x => x.Companies).Returns(mockQueryable.Object);
        _mockMapper.Setup(x => x.Map<IEnumerable<CompanyResponse>>(It.IsAny<List<Company>>()))
            .Returns(new List<CompanyResponse>());

        // Act
        var result = await _companyService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockMapper.Verify(x => x.Map<IEnumerable<CompanyResponse>>(It.IsAny<List<Company>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_MultipleCompanies_ReturnsAllCompanies()
    {
        // Arrange
        var companies = new List<Company>
        {
            new() { Id = Guid.NewGuid(), Name = "Company 1", Auth0UserId = "user1" },
            new() { Id = Guid.NewGuid(), Name = "Company 2", Auth0UserId = "user2" },
            new() { Id = Guid.NewGuid(), Name = "Company 3", Auth0UserId = "user3" }
        };

        var expectedResponses = companies.Select(c => new CompanyResponse
        {
            Id = c.Id,
            Name = c.Name,
            Auth0UserId = c.Auth0UserId
        }).ToList();

        var mockQueryable = companies.AsQueryable().BuildMockDbSet();

        _mockDbContext.Setup(x => x.Companies).Returns(mockQueryable.Object);
        _mockMapper.Setup(x => x.Map<IEnumerable<CompanyResponse>>(It.IsAny<List<Company>>()))
            .Returns(expectedResponses);

        // Act
        var result = await _companyService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, r => r.Name == "Company 1");
        Assert.Contains(result, r => r.Name == "Company 2");
        Assert.Contains(result, r => r.Name == "Company 3");

        _mockMapper.Verify(x => x.Map<IEnumerable<CompanyResponse>>(It.IsAny<List<Company>>()), Times.Once);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ExistingCompany_UpdatesAndReturnsCompanyResponse()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var originalCompany = new Company
        {
            Id = companyId,
            Name = "Original Name",
            About = "Original About",
            Auth0UserId = "test-user",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateRequest = new UpdateCompanyRequest
        {
            Name = "Updated Name",
            About = "Updated About"
        };

        var expectedResponse = new CompanyResponse
        {
            Id = companyId,
            Name = "Updated Name",
            About = "Updated About"
        };

        _mockCompanyDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>())).ReturnsAsync(originalCompany);
        _mockMapper.Setup(x => x.Map(updateRequest, It.IsAny<Company>()))
            .Callback<UpdateCompanyRequest, Company>((req, comp) =>
            {
                comp.Name = req.Name ?? comp.Name;
                comp.About = req.About ?? comp.About;
            });
        _mockMapper.Setup(x => x.Map<CompanyResponse>(It.IsAny<Company>())).Returns(expectedResponse);
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _companyService.UpdateAsync(companyId, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
        Assert.NotNull(originalCompany.UpdatedAt);

        _mockCompanyDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>()), Times.Once);
        _mockMapper.Verify(x => x.Map(updateRequest, It.IsAny<Company>()), Times.Once);
        _mockMapper.Verify(x => x.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
        _mockCompanyDbSet.Verify(x => x.Update(It.IsAny<Company>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistentCompany_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateCompanyRequest { Name = "New Name" };

        _mockCompanyDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>())).ReturnsAsync((Company?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _companyService.UpdateAsync(nonExistentId, updateRequest));

        Assert.Equal("Company", exception.EntityName);
        Assert.Equal(nonExistentId, exception.EntityId);

        _mockCompanyDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>()), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ExistingCompany_DeletesCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company
        {
            Id = companyId,
            Name = "Company to Delete",
            Auth0UserId = "test-user"
        };

        _mockCompanyDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>())).ReturnsAsync(company);
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _companyService.DeleteAsync(companyId);

        // Assert
        _mockCompanyDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>()), Times.Once);
        _mockCompanyDbSet.Verify(x => x.Remove(It.IsAny<Company>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentCompany_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockCompanyDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>())).ReturnsAsync((Company?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _companyService.DeleteAsync(nonExistentId));

        Assert.Equal("Company", exception.EntityName);
        Assert.Equal(nonExistentId, exception.EntityId);

        _mockCompanyDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>()), Times.Once);
    }

    #endregion
}
