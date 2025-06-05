using System;

namespace SkillBridge.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when a business rule validation fails.
/// </summary>
public class BusinessRuleValidationException : Exception
{
    public string RuleName { get; }

    public BusinessRuleValidationException(string ruleName, string message)
        : base(message)
    {
        RuleName = ruleName;
    }

    public BusinessRuleValidationException(string ruleName, string message, Exception innerException)
        : base(message, innerException)
    {
        RuleName = ruleName;
    }
}
