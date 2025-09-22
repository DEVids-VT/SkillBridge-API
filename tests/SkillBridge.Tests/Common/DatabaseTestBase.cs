using Microsoft.EntityFrameworkCore;
using SkillBridge.Data;

namespace SkillBridge.Tests.Common;

/// <summary>
/// Base class for database-related tests
/// </summary>
public abstract class DatabaseTestBase : IDisposable
{
    protected readonly AppDbContext DbContext;

    protected DatabaseTestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"SkillBridgeTest_{Guid.NewGuid()}")
            .Options;

        DbContext = new AppDbContext(options);
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
