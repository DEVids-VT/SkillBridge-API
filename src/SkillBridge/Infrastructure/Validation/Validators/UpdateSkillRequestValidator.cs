using FluentValidation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the UpdateSkillRequest class
/// </summary>
public class UpdateSkillRequestValidator : AbstractValidator<UpdateSkillRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSkillRequestValidator"/> class.
    /// </summary>
    public UpdateSkillRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required")
            .MaximumLength(100).WithMessage("Skill name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}
