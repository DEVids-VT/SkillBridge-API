using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Models.Entities;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.CurrentUser;
using SkillBridge.Services.File;

namespace SkillBridge.Services.Company;

/// <summary>
/// Implementation of the company service
/// </summary>
public class CompanyService : ICompanyService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<CompanyService> _logger;
    private readonly ICurrentUser _currentUser;
    private readonly IFileUploader _fileUploader;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyService"/> class.
    /// </summary>
    public CompanyService(
        AppDbContext dbContext, 
        IMapper mapper, 
        ILogger<CompanyService> logger,
        ICurrentUser currentUser,
        IFileUploader fileUploader)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
        _currentUser = currentUser;
        _fileUploader = fileUploader;
    }

    /// <summary>
    /// Creates a new company for the current user
    /// </summary>
    public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest request)
    {
        _logger.LogInformation("Creating new company with name: {CompanyName}", request.Name);
        
        var company = _mapper.Map<Models.Entities.Company>(request);
        
        // Set the Auth0 user ID from the current user instead of from request
        company.Auth0UserId = _currentUser.GetUserId();
        
        await _dbContext.Companies.AddAsync(company);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Company created successfully with ID: {CompanyId}", company.Id);
        
        return _mapper.Map<CompanyResponse>(company);
    }

    /// <summary>
    /// Gets a company by ID
    /// </summary>
    public async Task<CompanyResponse> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving company with ID: {CompanyId}", id);
        
        var company = await _dbContext.Companies.FindAsync(id);
        
        if (company == null)
        {
            _logger.LogWarning("Company with ID {CompanyId} not found", id);
            throw new EntityNotFoundException(nameof(Models.Entities.Company), id);
        }
        
        _logger.LogInformation("Company found: {CompanyName}", company.Name);
        
        return _mapper.Map<CompanyResponse>(company);
    }

    /// <summary>
    /// Gets the company of the currently logged in user
    /// </summary>
    public async Task<CompanyResponse> GetMyCompanyAsync(string? userId = null)
    {
        // If userId is not provided, use the current user's ID
        var auth0UserId = userId ?? _currentUser.GetUserId();
        
        _logger.LogInformation("Retrieving company for user ID: {Auth0UserId}", auth0UserId);
        
        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Auth0UserId == auth0UserId);
        
        if (company == null)
        {
            _logger.LogWarning("Company for user ID {Auth0UserId} not found", auth0UserId);
            throw new EntityNotFoundException("Company", $"for user {auth0UserId}", 
                $"No company found for user ID {auth0UserId}");
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
    public async Task<CompanyResponse> UpdateAsync(Guid id, UpdateCompanyRequest request)
    {
        _logger.LogInformation("Updating company with ID: {CompanyId}", id);
        
        var company = await _dbContext.Companies.FindAsync(id);
        
        if (company == null)
        {
            _logger.LogWarning("Company with ID {CompanyId} not found", id);
            throw new EntityNotFoundException(nameof(Models.Entities.Company), id);
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
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting company with ID: {CompanyId}", id);
        
        var company = await _dbContext.Companies.FindAsync(id);
        
        if (company == null)
        {
            _logger.LogWarning("Company with ID {CompanyId} not found", id);
            throw new EntityNotFoundException(nameof(Models.Entities.Company), id);
        }
        
        _dbContext.Companies.Remove(company);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Company deleted successfully: {CompanyId}", id);
    }

    public async Task<CompanyResponse> UpdateCompanyLogoAsync(Guid id, UpdateCompanyLogoRequest request)
    {
        //var auth0UserId = id ?? _currentUser.GetUserId();
        _logger.LogInformation("Updating profile with ID: {UserProfileId}", id);

        var companyProfile = await _dbContext.Companies.FindAsync(id);

        if (companyProfile == null)
        {
            _logger.LogWarning("Profile with ID {ProfileId} not found", id);
            throw new EntityNotFoundException(nameof(Models.Entities.Company), id);
        }


        if (!string.IsNullOrEmpty(companyProfile.LogoUrl))
        {
            await _fileUploader.DeleteFileAsync(companyProfile.LogoUrl, Models.Enums.FileType.Image);
            companyProfile.LogoUrl = null;
        }
        if (request.LogoUrl != null)
        {
            companyProfile.LogoUrl = await _fileUploader.UploadFileAsync(request.LogoUrl, Models.Enums.FileType.Image);
        }

        companyProfile.UpdatedAt = DateTime.UtcNow;

        _dbContext.Companies.Update(companyProfile);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Profile updated successfully with Id: {ProfileId}", companyProfile.Id);

        return _mapper.Map<CompanyResponse>(companyProfile);
    }
}
