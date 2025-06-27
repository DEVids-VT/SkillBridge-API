using FluentValidation;
using SkillBridge.Infrastructure.Validation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the CreateProjectAssignmentRequest class
/// </summary>
public class CreateProjectAssignmentRequestValidator : AbstractValidator<CreateProjectAssignmentRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectAssignmentRequestValidator"/> class.
    /// </summary>
    public CreateProjectAssignmentRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Project assignment title is required")
            .MaximumLength(ValidationConstants.ProjectAssignment.TitleMaxLength)
            .WithMessage($"Project assignment title cannot exceed {ValidationConstants.ProjectAssignment.TitleMaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.ProjectAssignment.DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {ValidationConstants.ProjectAssignment.DescriptionMaxLength} characters");

        RuleFor(x => x.Deadline)
            .NotEmpty().WithMessage("Deadline is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future");
    }
}
