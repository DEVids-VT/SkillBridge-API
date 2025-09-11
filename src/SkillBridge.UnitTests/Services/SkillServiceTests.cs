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
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.Skill;

namespace SkillBridge.UnitTests.Services;

public class SkillServiceTests
{
    private readonly Mock<AppDbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<SkillService>> _mockLogger;
    private readonly SkillService _skillService;

    public SkillServiceTests()
    {
        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<SkillService>>();

        _skillService = new SkillService(
            _mockDbContext.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsSkillResponse()
    {
        // Arrange
        var request = CreateValidCreateSkillRequest();
        var skillEntity = CreateSkillEntity();
        var expectedResponse = CreateSkillResponse();

        var skills = new List<Models.Entities.Skill>();
        var mockSkillDbSet = skills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        _mockMapper.Setup(x => x.Map<Models.Entities.Skill>(request)).Returns(skillEntity);
        _mockMapper.Setup(x => x.Map<SkillResponse>(skillEntity)).Returns(expectedResponse);

        // Act
        var result = await _skillService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
        Assert.Equal(expectedResponse.Description, result.Description);

        _mockMapper.Verify(x => x.Map<Models.Entities.Skill>(request), Times.Once);
        _mockMapper.Verify(x => x.Map<SkillResponse>(skillEntity), Times.Once);
        mockSkillDbSet.Verify(x => x.AddAsync(skillEntity, default), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsSkillResponse()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skillEntity = CreateSkillEntity(skillId);
        var expectedResponse = CreateSkillResponse(skillId);

        var skills = new List<Models.Entities.Skill>();
        var mockSkillDbSet = skills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        mockSkillDbSet.Setup(x => x.FindAsync(skillId)).ReturnsAsync(skillEntity);
        _mockMapper.Setup(x => x.Map<SkillResponse>(skillEntity)).Returns(expectedResponse);

        // Act
        var result = await _skillService.GetByIdAsync(skillId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
        Assert.Equal(expectedResponse.Description, result.Description);

        mockSkillDbSet.Verify(x => x.FindAsync(skillId), Times.Once);
        _mockMapper.Verify(x => x.Map<SkillResponse>(skillEntity), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var skillId = Guid.NewGuid();

        var skills = new List<Models.Entities.Skill>();
        var mockSkillDbSet = skills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        mockSkillDbSet.Setup(x => x.FindAsync(skillId)).ReturnsAsync((Models.Entities.Skill?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _skillService.GetByIdAsync(skillId));

        Assert.Equal("Skill", exception.EntityName);
        Assert.Equal(skillId, exception.EntityId);

        mockSkillDbSet.Verify(x => x.FindAsync(skillId), Times.Once);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllSkills()
    {
        // Arrange
        var skillEntities = new List<Models.Entities.Skill>
        {
            CreateSkillEntity(Guid.NewGuid(), "C#"),
            CreateSkillEntity(Guid.NewGuid(), "JavaScript"),
            CreateSkillEntity(Guid.NewGuid(), "Python")
        };

        var mockSkillDbSet = skillEntities.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        var expectedResponses = skillEntities.Select(s => new SkillResponse
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description
        }).ToList();

        _mockMapper.Setup(x => x.Map<IEnumerable<SkillResponse>>(skillEntities))
            .Returns(expectedResponses);

        // Act
        var result = await _skillService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, s => s.Name == "C#");
        Assert.Contains(result, s => s.Name == "JavaScript");
        Assert.Contains(result, s => s.Name == "Python");

        _mockMapper.Verify(x => x.Map<IEnumerable<SkillResponse>>(skillEntities), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_NoSkills_ReturnsEmptyCollection()
    {
        // Arrange
        var skillEntities = new List<Models.Entities.Skill>();

        var mockSkillDbSet = skillEntities.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        _mockMapper.Setup(x => x.Map<IEnumerable<SkillResponse>>(skillEntities))
            .Returns(new List<SkillResponse>());

        // Act
        var result = await _skillService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockMapper.Verify(x => x.Map<IEnumerable<SkillResponse>>(skillEntities), Times.Once);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedSkillResponse()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var request = CreateValidUpdateSkillRequest();
        var skillEntity = CreateSkillEntity(skillId);
        var expectedResponse = CreateSkillResponse(skillId);

        var skills = new List<Models.Entities.Skill>();
        var mockSkillDbSet = skills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        mockSkillDbSet.Setup(x => x.FindAsync(skillId)).ReturnsAsync(skillEntity);
        _mockMapper.Setup(x => x.Map<SkillResponse>(skillEntity)).Returns(expectedResponse);

        // Act
        var result = await _skillService.UpdateAsync(skillId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);

        mockSkillDbSet.Verify(x => x.FindAsync(skillId), Times.Once);
        _mockMapper.Verify(x => x.Map(request, skillEntity), Times.Once);
        mockSkillDbSet.Verify(x => x.Update(skillEntity), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        _mockMapper.Verify(x => x.Map<SkillResponse>(skillEntity), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SkillNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var request = CreateValidUpdateSkillRequest();

        var skills = new List<Models.Entities.Skill>();
        var mockSkillDbSet = skills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        mockSkillDbSet.Setup(x => x.FindAsync(skillId)).ReturnsAsync((Models.Entities.Skill?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _skillService.UpdateAsync(skillId, request));

        Assert.Equal("Skill", exception.EntityName);
        Assert.Equal(skillId, exception.EntityId);

        mockSkillDbSet.Verify(x => x.FindAsync(skillId), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ValidId_DeletesSkill()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skillEntity = CreateSkillEntity(skillId);

        var skills = new List<Models.Entities.Skill>();
        var mockSkillDbSet = skills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        mockSkillDbSet.Setup(x => x.FindAsync(skillId)).ReturnsAsync(skillEntity);

        // Act
        await _skillService.DeleteAsync(skillId);

        // Assert
        mockSkillDbSet.Verify(x => x.FindAsync(skillId), Times.Once);
        mockSkillDbSet.Verify(x => x.Remove(skillEntity), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SkillNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var skillId = Guid.NewGuid();

        var skills = new List<Models.Entities.Skill>();
        var mockSkillDbSet = skills.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        mockSkillDbSet.Setup(x => x.FindAsync(skillId)).ReturnsAsync((Models.Entities.Skill?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _skillService.DeleteAsync(skillId));

        Assert.Equal("Skill", exception.EntityName);
        Assert.Equal(skillId, exception.EntityId);

        mockSkillDbSet.Verify(x => x.FindAsync(skillId), Times.Once);
    }

    #endregion

    #region ValidateSkillsExistAsync Tests

    [Fact]
    public async Task ValidateSkillsExistAsync_AllSkillsExist_ReturnsSkillIds()
    {
        // Arrange
        var skillId1 = Guid.NewGuid();
        var skillId2 = Guid.NewGuid();
        var skillIds = new List<Guid> { skillId1, skillId2 };

        var skillEntities = new List<Models.Entities.Skill>
        {
            CreateSkillEntity(skillId1, "C#"),
            CreateSkillEntity(skillId2, "JavaScript")
        };

        var mockSkillDbSet = skillEntities.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        // Act
        var result = await _skillService.ValidateSkillsExistAsync(skillIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(skillId1, result);
        Assert.Contains(skillId2, result);
    }

    [Fact]
    public async Task ValidateSkillsExistAsync_SomeSkillsMissing_ThrowsEntityNotFoundException()
    {
        // Arrange
        var skillId1 = Guid.NewGuid();
        var skillId2 = Guid.NewGuid();
        var missingSkillId = Guid.NewGuid();
        var skillIds = new List<Guid> { skillId1, skillId2, missingSkillId };

        var skillEntities = new List<Models.Entities.Skill>
        {
            CreateSkillEntity(skillId1, "C#"),
            CreateSkillEntity(skillId2, "JavaScript")
            // Missing the third skill
        };

        var mockSkillDbSet = skillEntities.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _skillService.ValidateSkillsExistAsync(skillIds));

        Assert.Equal("Skill", exception.EntityName);
        Assert.Contains(missingSkillId.ToString(), exception.EntityId.ToString());
    }

    [Fact]
    public async Task ValidateSkillsExistAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var skillIds = new List<Guid>();

        // Act
        var result = await _skillService.ValidateSkillsExistAsync(skillIds);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ValidateSkillsExistAsync_NullList_ReturnsEmptyList()
    {
        // Act
        var result = await _skillService.ValidateSkillsExistAsync(null!);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetOrCreateSkillsByNameAsync Tests

    [Fact]
    public async Task GetOrCreateSkillsByNameAsync_AllSkillsExist_ReturnsExistingSkillIds()
    {
        // Arrange
        var skillNames = new List<string> { "C#", "JavaScript" };
        var skillEntities = new List<Models.Entities.Skill>
        {
            CreateSkillEntity(Guid.NewGuid(), "C#"),
            CreateSkillEntity(Guid.NewGuid(), "JavaScript")
        };

        var mockSkillDbSet = skillEntities.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        // Act
        var result = await _skillService.GetOrCreateSkillsByNameAsync(skillNames);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, id => Assert.Contains(skillEntities, s => s.Id == id));
    }

    [Fact]
    public async Task GetOrCreateSkillsByNameAsync_SomeSkillsMissing_CreatesNewSkills()
    {
        // Arrange
        var skillNames = new List<string> { "C#", "JavaScript", "Python" };
        var existingSkillEntities = new List<Models.Entities.Skill>
        {
            CreateSkillEntity(Guid.NewGuid(), "C#")
            // JavaScript and Python don't exist
        };

        var mockSkillDbSet = existingSkillEntities.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        // Act
        var result = await _skillService.GetOrCreateSkillsByNameAsync(skillNames);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        // Verify that AddRangeAsync was called for the new skills
        mockSkillDbSet.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Models.Entities.Skill>>(), default), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateSkillsByNameAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var skillNames = new List<string>();

        // Act
        var result = await _skillService.GetOrCreateSkillsByNameAsync(skillNames);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetOrCreateSkillsByNameAsync_NullList_ReturnsEmptyList()
    {
        // Act
        var result = await _skillService.GetOrCreateSkillsByNameAsync(null!);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetOrCreateSkillsByNameAsync_DuplicateNames_ReturnsUniqueSkills()
    {
        // Arrange
        var skillNames = new List<string> { "C#", "c#", "C#", "JavaScript" };
        var existingSkillEntities = new List<Models.Entities.Skill>
        {
            CreateSkillEntity(Guid.NewGuid(), "C#")
        };

        var mockSkillDbSet = existingSkillEntities.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        // Act
        var result = await _skillService.GetOrCreateSkillsByNameAsync(skillNames);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // C# (existing) and JavaScript (created)

        // Verify that only JavaScript was created (not duplicate C# entries)
        mockSkillDbSet.Verify(x => x.AddRangeAsync(
            It.Is<IEnumerable<Models.Entities.Skill>>(skills => skills.Count() == 1), 
            default), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateSkillsByNameAsync_WhitespaceAndEmptyNames_FiltersCorrectly()
    {
        // Arrange
        var skillNames = new List<string> { "C#", "  ", "", "JavaScript", null! };
        var existingSkillEntities = new List<Models.Entities.Skill>();

        var mockSkillDbSet = existingSkillEntities.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.Skills).Returns(mockSkillDbSet.Object);

        // Act
        var result = await _skillService.GetOrCreateSkillsByNameAsync(skillNames);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Only C# and JavaScript should be processed

        // Verify that exactly 2 skills were created (after filtering)
        mockSkillDbSet.Verify(x => x.AddRangeAsync(
            It.Is<IEnumerable<Models.Entities.Skill>>(skills => skills.Count() == 2), 
            default), Times.Once);
    }

    #endregion

    #region Private Helper Methods

    private CreateSkillRequest CreateValidCreateSkillRequest()
    {
        return new CreateSkillRequest
        {
            Name = "Test Skill",
            Description = "Test skill description"
        };
    }

    private UpdateSkillRequest CreateValidUpdateSkillRequest()
    {
        return new UpdateSkillRequest
        {
            Name = "Updated Skill",
            Description = "Updated skill description"
        };
    }

    private Models.Entities.Skill CreateSkillEntity(Guid? id = null, string? name = null)
    {
        return new Models.Entities.Skill
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? "Test Skill",
            Description = "Test skill description"
        };
    }

    private SkillResponse CreateSkillResponse(Guid? id = null, string? name = null)
    {
        return new SkillResponse
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? "Test Skill",
            Description = "Test skill description"
        };
    }

    #endregion
}