using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.Company;
using SkillBridge.Services.CurrentUser;

namespace SkillBridge.Controllers;

/// <summary>
/// Controller for company operations
/// </summary>
[ApiController]
[Route("api/c")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompaniesController"/> class.
    /// </summary>
    /// <param name="companyService">The company service</param>
    /// <param name="currentUser">The current user service</param>
    public CompaniesController(ICompanyService companyService, ICurrentUser currentUser)
    {
        _companyService = companyService;
        _currentUser = currentUser;
    }
    
    /// <summary>
    /// Creates a new company for the current user
    /// </summary>
    /// <param name="request">The company creation request</param>
    /// <returns>The created company</returns>
    //[Authorize(Policy = "CompanyScope")]
    [HttpPost]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
    {
        var company = await _companyService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
    }    /// <summary>
    /// Gets a company by ID
    /// </summary>
    /// <param name="id">The company ID</param>
    /// <returns>The company if found</returns>
    [HttpGet("id/{id:guid}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var company = await _companyService.GetByIdAsync(id);
        return Ok(company);
    }
      /// <summary>
    /// Gets the company of the current user or the specified user
    /// </summary>
    /// <param name="userId">Optional user ID (if not provided, uses the current user)</param>
    /// <returns>The company if found</returns>
    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyCompany([FromQuery] string? userId = null)
    {
        var company = await _companyService.GetMyCompanyAsync(userId);
        return Ok(company);
    }

    /// <summary>
    /// Gets all companies
    /// </summary>
    /// <returns>A list of all companies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var companies = await _companyService.GetAllAsync();
        return Ok(companies);
    }    /// <summary>
    /// Updates a company
    /// </summary>
    /// <param name="id">The company ID</param>
    /// <param name="request">The company update request</param>
    /// <returns>The updated company</returns>
    [Authorize(Policy = "Company")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyRequest request)
    {
        var company = await _companyService.UpdateAsync(id, request);
        return Ok(company);
    }    /// <summary>
    /// Deletes a company
    /// </summary>
    /// <param name="id">The company ID</param>
    /// <returns>No content if deleted successfully</returns>
    [Authorize(Policy = "CompaniesScope")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _companyService.DeleteAsync(id);
        return NoContent();
    }


    [Authorize(Policy = "Company")]
    [HttpPatch("{id?}/companyLogo")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCompanyLogo(Guid id, [FromForm] UpdateCompanyLogoRequest request)
    {
        var company = await _companyService.UpdateCompanyLogoAsync(id, request);
        return Ok(company);
    }
}
