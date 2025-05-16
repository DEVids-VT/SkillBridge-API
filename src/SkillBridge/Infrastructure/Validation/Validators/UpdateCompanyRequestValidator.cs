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

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}
