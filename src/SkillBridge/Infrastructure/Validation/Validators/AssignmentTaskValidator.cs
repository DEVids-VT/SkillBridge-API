using FluentValidation;
using SkillBridge.Models.Entities;

namespace SkillBridge.Infrastructure.Validation.Validators;

/// <summary>
/// Validator for the AssignmentTask entity
/// </summary>
public class AssignmentTaskValidator : AbstractValidator<AssignmentTask>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignmentTaskValidator"/> class.
    /// </summary>
    public AssignmentTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(ValidationConstants.AssignmentTask.TitleMaxLength)
            .WithMessage($"Task title cannot exceed {ValidationConstants.AssignmentTask.TitleMaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.AssignmentTask.DescriptionMaxLength)
            .WithMessage($"Task description cannot exceed {ValidationConstants.AssignmentTask.DescriptionMaxLength} characters");

        RuleFor(x => x.ProjectAssignmentId)
            .NotEmpty().WithMessage("Project assignment ID is required");

        RuleFor(x => x.Sequence)
            .GreaterThanOrEqualTo(0).WithMessage("Sequence must be a non-negative number");
    }
}