using FluentValidation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the CreateSkillRequest class
/// </summary>
public class CreateSkillRequestValidator : AbstractValidator<CreateSkillRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSkillRequestValidator"/> class.
    /// </summary>
    public CreateSkillRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required")
            .MaximumLength(100).WithMessage("Skill name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}
