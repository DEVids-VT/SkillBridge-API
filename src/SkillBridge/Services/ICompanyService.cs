using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services;

/// <summary>
/// Interface for the company service
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Creates a new company
    /// </summary>
    /// <param name="request">The company creation request</param>
    /// <returns>The created company</returns>
    Task<CompanyResponse> CreateAsync(CreateCompanyRequest request);
    
    /// <summary>
    /// Gets a company by ID
    /// </summary>
    /// <param name="id">The company ID</param>
    /// <returns>The company if found</returns>
    Task<CompanyResponse?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Gets a company by Auth0 user ID
    /// </summary>
    /// <param name="auth0UserId">The Auth0 user ID</param>
    /// <returns>The company if found</returns>
    Task<CompanyResponse?> GetByAuth0UserIdAsync(string auth0UserId);
    
    /// <summary>
    /// Gets all companies
    /// </summary>
    /// <returns>A list of all companies</returns>
    Task<IEnumerable<CompanyResponse>> GetAllAsync();
    
    /// <summary>
    /// Updates a company
    /// </summary>
    /// <param name="id">The company ID</param>
    /// <param name="request">The company update request</param>
    /// <returns>The updated company</returns>
    Task<CompanyResponse?> UpdateAsync(Guid id, UpdateCompanyRequest request);
    
    /// <summary>
    /// Deletes a company
    /// </summary>
    /// <param name="id">The company ID</param>
    /// <returns>True if company was deleted, false otherwise</returns>
    Task<bool> DeleteAsync(Guid id);
}
