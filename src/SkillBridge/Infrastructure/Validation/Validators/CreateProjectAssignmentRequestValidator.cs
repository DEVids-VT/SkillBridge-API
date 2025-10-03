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

        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("Project assignment summary is required")
            .MaximumLength(ValidationConstants.ProjectAssignment.SummaryMaxLength)
            .WithMessage($"Summary cannot exceed {ValidationConstants.ProjectAssignment.SummaryMaxLength} characters");

        RuleFor(x => x.LearningBenefits)
            .NotEmpty().WithMessage("Learning benefits are required")
            .MaximumLength(ValidationConstants.ProjectAssignment.LearningBenefitsMaxLength)
            .WithMessage($"Learning benefits cannot exceed {ValidationConstants.ProjectAssignment.LearningBenefitsMaxLength} characters");

        RuleFor(x => x.SuggestedApproach)
            .NotEmpty().WithMessage("Suggested approach is required")
            .MaximumLength(ValidationConstants.ProjectAssignment.SuggestedApproachMaxLength)
            .WithMessage($"Suggested approach cannot exceed {ValidationConstants.ProjectAssignment.SuggestedApproachMaxLength} characters");

        RuleFor(x => x.Duratoin)
            .NotEmpty().WithMessage("Duration is required")
            .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than zero")
            .LessThanOrEqualTo(TimeSpan.FromDays(30)).WithMessage("Duration cannot exceed 30 days");

    }
}
