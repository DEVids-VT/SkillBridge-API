using FluentValidation;
using SkillBridge.Infrastructure.Validation;
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
            .MaximumLength(ValidationConstants.Skill.NameMaxLength)
            .WithMessage($"Skill name cannot exceed {ValidationConstants.Skill.NameMaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.Skill.DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {ValidationConstants.Skill.DescriptionMaxLength} characters");
    }
}
