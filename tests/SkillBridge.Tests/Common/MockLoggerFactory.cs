using Microsoft.Extensions.Logging;
using Moq;

namespace SkillBridge.Tests.Common;

/// <summary>
/// Factory for creating mock loggers
/// </summary>
public static class MockLoggerFactory
{
    /// <summary>
    /// Creates a mock logger for the specified type
    /// </summary>
    /// <typeparam name="T">The type to create a logger for</typeparam>
    public static Mock<ILogger<T>> CreateLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }
    
    /// <summary>
    /// Verifies that a log message was written at the specified log level
    /// </summary>
    public static void VerifyLog<T>(Mock<ILogger<T>> loggerMock, LogLevel logLevel, string message, Times times)
    {
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == logLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            times);
    }
}
