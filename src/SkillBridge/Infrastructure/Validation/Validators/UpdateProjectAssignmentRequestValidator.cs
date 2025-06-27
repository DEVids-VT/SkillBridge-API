using FluentValidation;
using SkillBridge.Infrastructure.Validation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the UpdateProjectAssignmentRequest class
/// </summary>
public class UpdateProjectAssignmentRequestValidator : AbstractValidator<UpdateProjectAssignmentRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectAssignmentRequestValidator"/> class.
    /// </summary>
    public UpdateProjectAssignmentRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Project assignment title is required")
            .MaximumLength(ValidationConstants.ProjectAssignment.TitleMaxLength)
            .WithMessage($"Project assignment title cannot exceed {ValidationConstants.ProjectAssignment.TitleMaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.ProjectAssignment.DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {ValidationConstants.ProjectAssignment.DescriptionMaxLength} characters");

        RuleFor(x => x.Deadline)
            .NotEmpty().WithMessage("Deadline is required");
    }
}
