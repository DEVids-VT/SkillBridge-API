using FluentValidation;
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
            .MaximumLength(200).WithMessage("Project assignment title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Deadline)
            .NotEmpty().WithMessage("Deadline is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future");
    }
}
