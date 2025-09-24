using FluentValidation;
using SkillBridge.Infrastructure.Validation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the CreateCompanyRequest class
/// </summary>
public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCompanyRequestValidator"/> class.
    /// </summary>
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(ValidationConstants.Company.NameMaxLength)
            .WithMessage($"Company name cannot exceed {ValidationConstants.Company.NameMaxLength} characters");

        RuleFor(x => x.About)
            .NotEmpty().WithMessage("About text is required")
            .MaximumLength(ValidationConstants.Company.AboutMaxLength)
            .WithMessage($"About text cannot exceed {ValidationConstants.Company.AboutMaxLength} characters");

        RuleFor(x => x.LogoUrl)
            .Must(file => file == null || file.Length <= ValidationConstants.Company.CompanyLogoMaxBytes)
            .WithMessage($"Logo file cannot exceed {ValidationConstants.Company.CompanyLogoMaxBytes / 1024 / 1024} MB")
            .When(x => x.LogoUrl != null);


        RuleFor(x => x.BannerUrl)
            .MaximumLength(ValidationConstants.Company.BannerUrlMaxLength)
            .WithMessage($"Banner URL cannot exceed {ValidationConstants.Company.BannerUrlMaxLength} characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.BannerUrl))
            .WithMessage("Banner URL must be a valid URL");

        RuleFor(x => x.Activities)
            .NotEmpty().WithMessage("Activities are required")
            .MaximumLength(ValidationConstants.Company.ActivitiesMaxLength)
            .WithMessage($"Activities cannot exceed {ValidationConstants.Company.ActivitiesMaxLength} characters");

        RuleFor(x => x.Sector)
            .NotEmpty().WithMessage("Sector is required")
            .MaximumLength(ValidationConstants.Company.SectorMaxLength)
            .WithMessage($"Sector cannot exceed {ValidationConstants.Company.SectorMaxLength} characters");

        RuleFor(x => x.HeadOfficeLocation)
            .NotEmpty().WithMessage("Head office location is required")
            .MaximumLength(ValidationConstants.Company.HeadOfficeLocationMaxLength)
            .WithMessage($"Head office location cannot exceed {ValidationConstants.Company.HeadOfficeLocationMaxLength} characters");

        RuleFor(x => x.Technologies)
            .NotEmpty().WithMessage("Technologies are required")
            .MaximumLength(ValidationConstants.Company.TechnologiesMaxLength)
            .WithMessage($"Technologies list cannot exceed {ValidationConstants.Company.TechnologiesMaxLength} characters");

        RuleFor(x => x.YearEstablished)
            .GreaterThan(ValidationConstants.Company.YearEstablishedMinimum).When(x => x.YearEstablished.HasValue)
            .WithMessage($"Year established must be after {ValidationConstants.Company.YearEstablishedMinimum}")
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .When(x => x.YearEstablished.HasValue)
            .WithMessage("Year established cannot be in the future");

        RuleFor(x => x.BulgarianOfficeLocations)
            .NotEmpty().When(x => x.HasOfficesInBulgaria)
            .WithMessage("Bulgarian office locations are required when HasOfficesInBulgaria is true")
            .MaximumLength(ValidationConstants.Company.BulgarianOfficeLocationsMaxLength)
            .WithMessage($"Bulgarian office locations cannot exceed {ValidationConstants.Company.BulgarianOfficeLocationsMaxLength} characters");

        RuleFor(x => x.EmployeesInBulgaria)
            .GreaterThan(0).When(x => x.EmployeesInBulgaria.HasValue)
            .WithMessage("Number of employees in Bulgaria must be greater than 0")
            .LessThanOrEqualTo(x => x.EmployeesWorldwide)
            .When(x => x.EmployeesInBulgaria.HasValue)
            .WithMessage("Number of employees in Bulgaria cannot be greater than worldwide employees");

        RuleFor(x => x.EmployeesWorldwide)
            .NotEmpty().WithMessage("Number of employees worldwide is required")
            .GreaterThan(0).WithMessage("Number of employees worldwide must be greater than 0");

        RuleFor(x => x.WhyWorkWithUs)
            .MaximumLength(ValidationConstants.Company.WhyWorkWithUsMaxLength)
            .WithMessage($"Why work with us text cannot exceed {ValidationConstants.Company.WhyWorkWithUsMaxLength} characters");

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty().WithMessage("Website URL is required")
            .MaximumLength(ValidationConstants.Company.WebsiteUrlMaxLength)
            .WithMessage($"Website URL cannot exceed {ValidationConstants.Company.WebsiteUrlMaxLength} characters")
            .Must(BeAValidUrl).WithMessage("Website URL must be a valid URL");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Contact name is required")
            .MaximumLength(ValidationConstants.Company.ContactNameMaxLength)
            .WithMessage($"Contact name cannot exceed {ValidationConstants.Company.ContactNameMaxLength} characters");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required")
            .EmailAddress().WithMessage("Contact email must be a valid email address")
            .MaximumLength(ValidationConstants.Company.ContactEmailMaxLength)
            .WithMessage($"Contact email cannot exceed {ValidationConstants.Company.ContactEmailMaxLength} characters");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("Contact phone is required")
            .MaximumLength(ValidationConstants.Company.ContactPhoneMaxLength)
            .WithMessage($"Contact phone cannot exceed {ValidationConstants.Company.ContactPhoneMaxLength} characters")
            .Matches(@"^[\+]?[0-9\s\-\(\)]{5,20}$")
            .WithMessage("Contact phone must be a valid phone number");
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
