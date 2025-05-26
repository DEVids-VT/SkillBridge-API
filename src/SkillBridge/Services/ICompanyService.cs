using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services;

/// <summary>
/// Interface for the company service
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Creates a new company for the current user
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
    /// Gets the company of the currently logged in user
    /// </summary>
    /// <param name="userId">Optional user ID to get company for, defaults to current user</param>
    /// <returns>The company if found</returns>
    Task<CompanyResponse?> GetMyCompanyAsync(string? userId = null);
    
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
