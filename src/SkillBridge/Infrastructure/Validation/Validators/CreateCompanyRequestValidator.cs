using FluentValidation;
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
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Auth0UserId)
            .NotEmpty().WithMessage("Auth0 user ID is required")
            .MaximumLength(50).WithMessage("Auth0 user ID cannot exceed 50 characters");
    }
}
