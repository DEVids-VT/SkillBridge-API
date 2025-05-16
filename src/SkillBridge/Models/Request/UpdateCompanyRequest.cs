namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for updating a company
/// </summary>
public class UpdateCompanyRequest
{
    /// <summary>
    /// Gets or sets the name of the company
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the company
    /// </summary>
    public string? Description { get; set; }
}
