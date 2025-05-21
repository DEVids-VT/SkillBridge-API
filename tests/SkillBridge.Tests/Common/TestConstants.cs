namespace SkillBridge.Tests.Common;

/// <summary>
/// Constants used in tests
/// </summary>
public static class TestConstants
{
    public static class Skill
    {
        public const string ValidName = "C#";
        public const string ValidDescription = "C# programming language";
        public static readonly Guid ValidId = new("12345678-1234-1234-1234-123456789012");
    }
    
    public static class Company
    {
        public const string ValidName = "Test Company";
        public const string ValidDescription = "A company for testing";
        public static readonly Guid ValidId = new("22345678-1234-1234-1234-123456789012");
    }
    
    public static class ProjectAssignment
    {
        public const string ValidTitle = "Test Project";
        public const string ValidDescription = "A project for testing";
        public static readonly Guid ValidId = new("32345678-1234-1234-1234-123456789012");
    }
}
