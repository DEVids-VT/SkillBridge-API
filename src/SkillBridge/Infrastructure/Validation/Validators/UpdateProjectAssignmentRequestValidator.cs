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

        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Duration is required");
    }
}
