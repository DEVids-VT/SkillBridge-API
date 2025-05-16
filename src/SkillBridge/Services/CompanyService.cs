using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services;

/// <summary>
/// Implementation of the company service
/// </summary>
public class CompanyService : ICompanyService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<CompanyService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyService"/> class.
    /// </summary>
    public CompanyService(AppDbContext dbContext, IMapper mapper, ILogger<CompanyService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new company
    /// </summary>
    public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest request)
    {
        _logger.LogInformation("Creating new company with name: {CompanyName}", request.Name);
        
        var company = _mapper.Map<Company>(request);
        
        await _dbContext.Companies.AddAsync(company);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Company created successfully with ID: {CompanyId}", company.Id);
        
        return _mapper.Map<CompanyResponse>(company);
    }

    /// <summary>
    /// Gets a company by ID
    /// </summary>
    public async Task<CompanyResponse?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving company with ID: {CompanyId}", id);
        
        var company = await _dbContext.Companies.FindAsync(id);
        
        if (company == null)
        {
            _logger.LogWarning("Company with ID {CompanyId} not found", id);
            return null;
        }
        
        _logger.LogInformation("Company found: {CompanyName}", company.Name);
        
        return _mapper.Map<CompanyResponse>(company);
    }

    /// <summary>
    /// Gets a company by Auth0 user ID
    /// </summary>
    public async Task<CompanyResponse?> GetByAuth0UserIdAsync(string auth0UserId)
    {
        _logger.LogInformation("Retrieving company with Auth0 user ID: {Auth0UserId}", auth0UserId);
        
        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Auth0UserId == auth0UserId);
        
        if (company == null)
        {
            _logger.LogWarning("Company with Auth0 user ID {Auth0UserId} not found", auth0UserId);
            return null;
        }
        
        _logger.LogInformation("Company found: {CompanyName}", company.Name);
        
        return _mapper.Map<CompanyResponse>(company);
    }

    /// <summary>
    /// Gets all companies
    /// </summary>
    public async Task<IEnumerable<CompanyResponse>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all companies");
        
        var companies = await _dbContext.Companies.ToListAsync();
        
        _logger.LogInformation("Retrieved {CompanyCount} companies", companies.Count);
        
        return _mapper.Map<IEnumerable<CompanyResponse>>(companies);
    }

    /// <summary>
    /// Updates a company
    /// </summary>
    public async Task<CompanyResponse?> UpdateAsync(Guid id, UpdateCompanyRequest request)
    {
        _logger.LogInformation("Updating company with ID: {CompanyId}", id);
        
        var company = await _dbContext.Companies.FindAsync(id);
        
        if (company == null)
        {
            _logger.LogWarning("Company with ID {CompanyId} not found", id);
            return null;
        }
        
        _mapper.Map(request, company);
        company.UpdatedAt = DateTime.UtcNow;
        
        _dbContext.Companies.Update(company);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Company updated successfully: {CompanyName}", company.Name);
        
        return _mapper.Map<CompanyResponse>(company);
    }

    /// <summary>
    /// Deletes a company
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting company with ID: {CompanyId}", id);
        
        var company = await _dbContext.Companies.FindAsync(id);
        
        if (company == null)
        {
            _logger.LogWarning("Company with ID {CompanyId} not found", id);
            return false;
        }
        
        _dbContext.Companies.Remove(company);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Company deleted successfully: {CompanyId}", id);
        
        return true;
    }
}
