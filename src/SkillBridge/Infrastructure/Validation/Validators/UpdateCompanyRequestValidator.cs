using FluentValidation;
using SkillBridge.Infrastructure.Validation;
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
            .MaximumLength(ValidationConstants.Company.NameMaxLength)
            .WithMessage($"Company name cannot exceed {ValidationConstants.Company.NameMaxLength} characters");

        RuleFor(x => x.About)
            .MaximumLength(ValidationConstants.Company.AboutMaxLength)
            .WithMessage($"About text cannot exceed {ValidationConstants.Company.AboutMaxLength} characters");

        RuleFor(x => x.LogoUrl)
            .MaximumLength(ValidationConstants.Company.LogoUrlMaxLength)
            .WithMessage($"Logo URL cannot exceed {ValidationConstants.Company.LogoUrlMaxLength} characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.LogoUrl))
            .WithMessage("Logo URL must be a valid URL");

        RuleFor(x => x.BannerUrl)
            .MaximumLength(ValidationConstants.Company.BannerUrlMaxLength)
            .WithMessage($"Banner URL cannot exceed {ValidationConstants.Company.BannerUrlMaxLength} characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.BannerUrl))
            .WithMessage("Banner URL must be a valid URL");

        RuleFor(x => x.Activities)
            .MaximumLength(ValidationConstants.Company.ActivitiesMaxLength)
            .WithMessage($"Activities cannot exceed {ValidationConstants.Company.ActivitiesMaxLength} characters");

        RuleFor(x => x.Sector)
            .MaximumLength(ValidationConstants.Company.SectorMaxLength)
            .WithMessage($"Sector cannot exceed {ValidationConstants.Company.SectorMaxLength} characters");

        RuleFor(x => x.HeadOfficeLocation)
            .MaximumLength(ValidationConstants.Company.HeadOfficeLocationMaxLength)
            .WithMessage($"Head office location cannot exceed {ValidationConstants.Company.HeadOfficeLocationMaxLength} characters");

        RuleFor(x => x.Technologies)
            .MaximumLength(ValidationConstants.Company.TechnologiesMaxLength)
            .WithMessage($"Technologies list cannot exceed {ValidationConstants.Company.TechnologiesMaxLength} characters");

        RuleFor(x => x.YearEstablished)
            .GreaterThan(ValidationConstants.Company.YearEstablishedMinimum).When(x => x.YearEstablished.HasValue)
            .WithMessage($"Year established must be after {ValidationConstants.Company.YearEstablishedMinimum}")
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .When(x => x.YearEstablished.HasValue)
            .WithMessage("Year established cannot be in the future");

        RuleFor(x => x.BulgarianOfficeLocations)
            .NotEmpty().When(x => x.HasOfficesInBulgaria == true)
            .WithMessage("Bulgarian office locations are required when HasOfficesInBulgaria is true")
            .MaximumLength(ValidationConstants.Company.BulgarianOfficeLocationsMaxLength)
            .WithMessage($"Bulgarian office locations cannot exceed {ValidationConstants.Company.BulgarianOfficeLocationsMaxLength} characters");

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
            .MaximumLength(ValidationConstants.Company.WhyWorkWithUsMaxLength)
            .WithMessage($"Why work with us text cannot exceed {ValidationConstants.Company.WhyWorkWithUsMaxLength} characters");

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(ValidationConstants.Company.WebsiteUrlMaxLength)
            .WithMessage($"Website URL cannot exceed {ValidationConstants.Company.WebsiteUrlMaxLength} characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.WebsiteUrl))
            .WithMessage("Website URL must be a valid URL");

        RuleFor(x => x.ContactInfo)
            .MaximumLength(ValidationConstants.Company.ContactInfoMaxLength)
            .WithMessage($"Contact information cannot exceed {ValidationConstants.Company.ContactInfoMaxLength} characters");
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
