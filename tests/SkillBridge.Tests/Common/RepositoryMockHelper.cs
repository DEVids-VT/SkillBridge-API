using Moq;
using SkillBridge.Data;
using SkillBridge.Models.Entities;

namespace SkillBridge.Tests.Common;

/// <summary>
/// Helper class for mocking repository operations
/// </summary>
public static class RepositoryMockHelper
{
    /// <summary>
    /// Sets up a mock for DbContext.Skills, returning the specified skills for Find operations
    /// </summary>
    public static void SetupSkillFind(Mock<AppDbContext> dbContextMock, Skill? skillToReturn)
    {
        var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Skill>>();
        
        if (skillToReturn != null)
        {
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(skillToReturn);
        }
        else
        {            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((Skill?)null);
        }
        
        dbContextMock.Setup(m => m.Skills).Returns(mockSet.Object);
    }
    
    /// <summary>
    /// Sets up a mock for DbContext.Skills, returning the specified skills for queries
    /// </summary>
    public static void SetupSkillQuery(Mock<AppDbContext> dbContextMock, IQueryable<Skill> skills)
    {
        var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Skill>>();
        
        mockSet.As<IQueryable<Skill>>().Setup(m => m.Provider).Returns(skills.Provider);
        mockSet.As<IQueryable<Skill>>().Setup(m => m.Expression).Returns(skills.Expression);
        mockSet.As<IQueryable<Skill>>().Setup(m => m.ElementType).Returns(skills.ElementType);
        mockSet.As<IQueryable<Skill>>().Setup(m => m.GetEnumerator()).Returns(skills.GetEnumerator());
        
        dbContextMock.Setup(m => m.Skills).Returns(mockSet.Object);
    }
}
