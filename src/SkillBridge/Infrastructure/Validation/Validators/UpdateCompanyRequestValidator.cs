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
            .NotEmpty().When(x => x.Name != null).WithMessage("Company name cannot be empty")
            .MaximumLength(ValidationConstants.Company.NameMaxLength)
            .WithMessage($"Company name cannot exceed {ValidationConstants.Company.NameMaxLength} characters");

        RuleFor(x => x.About)
            .NotEmpty().When(x => x.About != null).WithMessage("About text cannot be empty")
            .MaximumLength(ValidationConstants.Company.AboutMaxLength)
            .WithMessage($"About text cannot exceed {ValidationConstants.Company.AboutMaxLength} characters");

        RuleFor(x => x.BannerUrl)
            .MaximumLength(ValidationConstants.Company.BannerUrlMaxLength)
            .WithMessage($"Banner URL cannot exceed {ValidationConstants.Company.BannerUrlMaxLength} characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.BannerUrl))
            .WithMessage("Banner URL must be a valid URL");

        RuleFor(x => x.Activities)
            .NotEmpty().When(x => x.Activities != null).WithMessage("Activities cannot be empty")
            .MaximumLength(ValidationConstants.Company.ActivitiesMaxLength)
            .WithMessage($"Activities cannot exceed {ValidationConstants.Company.ActivitiesMaxLength} characters");

        RuleFor(x => x.Sector)
            .NotEmpty().When(x => x.Sector != null).WithMessage("Sector cannot be empty")
            .MaximumLength(ValidationConstants.Company.SectorMaxLength)
            .WithMessage($"Sector cannot exceed {ValidationConstants.Company.SectorMaxLength} characters");

        RuleFor(x => x.HeadOfficeLocation)
            .NotEmpty().When(x => x.HeadOfficeLocation != null).WithMessage("Head office location cannot be empty")
            .MaximumLength(ValidationConstants.Company.HeadOfficeLocationMaxLength)
            .WithMessage($"Head office location cannot exceed {ValidationConstants.Company.HeadOfficeLocationMaxLength} characters");

        RuleFor(x => x.Technologies)
            .NotEmpty().When(x => x.Technologies != null).WithMessage("Technologies cannot be empty")
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
            .NotEmpty().When(x => x.WebsiteUrl != null).WithMessage("Website URL cannot be empty")
            .MaximumLength(ValidationConstants.Company.WebsiteUrlMaxLength)
            .WithMessage($"Website URL cannot exceed {ValidationConstants.Company.WebsiteUrlMaxLength} characters")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.WebsiteUrl))
            .WithMessage("Website URL must be a valid URL");

        RuleFor(x => x.ContactName)
            .NotEmpty().When(x => x.ContactName != null).WithMessage("Contact name cannot be empty")
            .MaximumLength(ValidationConstants.Company.ContactNameMaxLength)
            .WithMessage($"Contact name cannot exceed {ValidationConstants.Company.ContactNameMaxLength} characters");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().When(x => x.ContactEmail != null).WithMessage("Contact email cannot be empty")
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail))
            .WithMessage("Contact email must be a valid email address")
            .MaximumLength(ValidationConstants.Company.ContactEmailMaxLength)
            .WithMessage($"Contact email cannot exceed {ValidationConstants.Company.ContactEmailMaxLength} characters");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().When(x => x.ContactPhone != null).WithMessage("Contact phone cannot be empty")
            .MaximumLength(ValidationConstants.Company.ContactPhoneMaxLength)
            .WithMessage($"Contact phone cannot exceed {ValidationConstants.Company.ContactPhoneMaxLength} characters")
            .Matches(@"^[\+]?[0-9\s\-\(\)]{5,20}$").When(x => !string.IsNullOrEmpty(x.ContactPhone))
            .WithMessage("Contact phone must be a valid phone number");
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
