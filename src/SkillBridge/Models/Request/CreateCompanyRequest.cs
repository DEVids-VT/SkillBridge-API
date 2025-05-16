namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for creating a new company
/// </summary>
public class CreateCompanyRequest
{
    /// <summary>
    /// Gets or sets the name of the company
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the company
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the Auth0 user ID of the company owner
    /// </summary>
    public string Auth0UserId { get; set; } = string.Empty;
}
