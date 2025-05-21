using FluentValidation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the UpdateCompanyRequest class
/// </summary>
public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCompanyRequestValidator"/> class.
    /// </summary>
    public UpdateCompanyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters");

        RuleFor(x => x.About)
            .MaximumLength(2000).WithMessage("About text cannot exceed 2000 characters");

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500).WithMessage("Logo URL cannot exceed 500 characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.LogoUrl))
            .WithMessage("Logo URL must be a valid URL");

        RuleFor(x => x.BannerUrl)
            .MaximumLength(500).WithMessage("Banner URL cannot exceed 500 characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.BannerUrl))
            .WithMessage("Banner URL must be a valid URL");

        RuleFor(x => x.Activities)
            .MaximumLength(200).WithMessage("Activities cannot exceed 200 characters");

        RuleFor(x => x.Sector)
            .MaximumLength(100).WithMessage("Sector cannot exceed 100 characters");

        RuleFor(x => x.HeadOfficeLocation)
            .MaximumLength(200).WithMessage("Head office location cannot exceed 200 characters");

        RuleFor(x => x.Technologies)
            .MaximumLength(500).WithMessage("Technologies list cannot exceed 500 characters");

        RuleFor(x => x.YearEstablished)
            .GreaterThan(1800).When(x => x.YearEstablished.HasValue)
            .WithMessage("Year established must be after 1800")
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .When(x => x.YearEstablished.HasValue)
            .WithMessage("Year established cannot be in the future");

        RuleFor(x => x.BulgarianOfficeLocations)
            .NotEmpty().When(x => x.HasOfficesInBulgaria == true)
            .WithMessage("Bulgarian office locations are required when HasOfficesInBulgaria is true")
            .MaximumLength(500).WithMessage("Bulgarian office locations cannot exceed 500 characters");

        RuleFor(x => x.EmployeesInBulgaria)
            .GreaterThan(0).When(x => x.EmployeesInBulgaria.HasValue)
            .WithMessage("Number of employees in Bulgaria must be greater than 0")
            .LessThanOrEqualTo(x => x.EmployeesWorldwide ?? int.MaxValue)
            .When(x => x.EmployeesInBulgaria.HasValue && x.EmployeesWorldwide.HasValue)
            .WithMessage("Number of employees in Bulgaria cannot be greater than worldwide employees");

        RuleFor(x => x.EmployeesWorldwide)
            .GreaterThan(0).When(x => x.EmployeesWorldwide.HasValue)
            .WithMessage("Number of employees worldwide must be greater than 0");

        RuleFor(x => x.WhyWorkWithUs)
            .MaximumLength(2000).WithMessage("Why work with us text cannot exceed 2000 characters");

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(500).WithMessage("Website URL cannot exceed 500 characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.WebsiteUrl))
            .WithMessage("Website URL must be a valid URL");

        RuleFor(x => x.ContactInfo)
            .MaximumLength(500).WithMessage("Contact information cannot exceed 500 characters");
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
