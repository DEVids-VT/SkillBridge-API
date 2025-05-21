namespace SkillBridge.Models.Request;

/// <summary>
/// Request model for updating a company
/// </summary>
public class UpdateCompanyRequest
{
    /// <summary>
    /// Gets or sets the name of the company
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the about text of the company
    /// </summary>
    public string? About { get; set; }
    
    /// <summary>
    /// Gets or sets the URL to the company logo
    /// </summary>
    public string? LogoUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the URL to the company banner
    /// </summary>
    public string? BannerUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the company's activities (e.g., Product company)
    /// </summary>
    public string? Activities { get; set; }
    
    /// <summary>
    /// Gets or sets the company's sector (e.g., IoT)
    /// </summary>
    public string? Sector { get; set; }
    
    /// <summary>
    /// Gets or sets the location of the head office (e.g., Sofia, Bulgaria)
    /// </summary>
    public string? HeadOfficeLocation { get; set; }
    
    /// <summary>
    /// Gets or sets the list of technologies used by the company
    /// </summary>
    public string? Technologies { get; set; }
    
    /// <summary>
    /// Gets or sets the year the company was established
    /// </summary>
    public int? YearEstablished { get; set; }
    
    /// <summary>
    /// Gets or sets whether the company has offices in Bulgaria
    /// </summary>
    public bool? HasOfficesInBulgaria { get; set; }
    
    /// <summary>
    /// Gets or sets the locations of offices in Bulgaria
    /// </summary>
    public string? BulgarianOfficeLocations { get; set; }
    
    /// <summary>
    /// Gets or sets the number of employees in Bulgaria
    /// </summary>
    public int? EmployeesInBulgaria { get; set; }
    
    /// <summary>
    /// Gets or sets the number of employees worldwide
    /// </summary>
    public int? EmployeesWorldwide { get; set; }
    
    /// <summary>
    /// Gets or sets the "Why work with us" text
    /// </summary>
    public string? WhyWorkWithUs { get; set; }
    
    /// <summary>
    /// Gets or sets the website URL
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the contacts information
    /// </summary>
    public string? ContactInfo { get; set; }
}
