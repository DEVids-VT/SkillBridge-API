using FluentValidation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the UpdateAssignmentTaskRequest class
/// </summary>
public class UpdateAssignmentTaskRequestValidator : AbstractValidator<UpdateAssignmentTaskRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAssignmentTaskRequestValidator"/> class.
    /// </summary>
    public UpdateAssignmentTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(ValidationConstants.AssignmentTask.TitleMaxLength)
            .WithMessage($"Task title cannot exceed {ValidationConstants.AssignmentTask.TitleMaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.AssignmentTask.DescriptionMaxLength)
            .WithMessage($"Task description cannot exceed {ValidationConstants.AssignmentTask.DescriptionMaxLength} characters");

        RuleFor(x => x.Sequence)
            .GreaterThanOrEqualTo(0).WithMessage("Sequence must be a non-negative number");
    }
}