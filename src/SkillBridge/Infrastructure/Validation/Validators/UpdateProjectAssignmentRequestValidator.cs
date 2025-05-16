using FluentValidation;
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
            .MaximumLength(200).WithMessage("Project assignment title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Deadline)
            .NotEmpty().WithMessage("Deadline is required");
    }
}
