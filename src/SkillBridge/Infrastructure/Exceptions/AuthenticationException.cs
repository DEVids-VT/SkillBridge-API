using System;

namespace SkillBridge.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when authentication or authorization fails.
/// </summary>
public class AuthenticationException : Exception
{
    public AuthenticationException(string message)
        : base(message)
    {
    }

    public AuthenticationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
