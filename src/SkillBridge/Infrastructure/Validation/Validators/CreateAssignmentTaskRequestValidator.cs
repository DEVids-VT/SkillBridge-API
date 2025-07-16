using FluentValidation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the CreateAssignmentTaskRequest class
/// </summary>
public class CreateAssignmentTaskRequestValidator : AbstractValidator<CreateAssignmentTaskRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAssignmentTaskRequestValidator"/> class.
    /// </summary>
    public CreateAssignmentTaskRequestValidator()
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