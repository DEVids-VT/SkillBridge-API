using Microsoft.Extensions.Logging;
using Moq;

namespace SkillBridge.Tests.Common;

/// <summary>
/// Base fixture for creating mocks and common test dependencies
/// </summary>
public abstract class BaseTestFixture
{
    protected Mock<ILogger<T>> CreateLoggerMock<T>()
    {
        return new Mock<ILogger<T>>();
    }
}
