using FluentValidation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators
{
    /// <summary>
    /// Validator for <see cref="CompleteUserProjectAssignmentRequest"/>.
    /// Ensures that the submission repository URL and submission notes meet length and format requirements.
    /// </summary>
    public class CompleteUserProjectAssignmentRequestValidator : AbstractValidator<CompleteUserProjectAssignmentRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteUserProjectAssignmentRequestValidator"/> class.
        /// Configures validation rules for the completion request, including repository URL and notes.
        /// </summary>
        public CompleteUserProjectAssignmentRequestValidator()
        {
            // GitHubConnection (optional)
            When(x => !string.IsNullOrWhiteSpace(x.SubmissionRepositoryUrl), () =>
            {
                RuleFor(x => x.SubmissionRepositoryUrl!)
                    .MaximumLength(ValidationConstants.UserProfile.GitHubConnectionMaxLength)
                    .WithMessage($"SubmissionRepositoryUrl cannot exceed {ValidationConstants.UserProfile.GitHubConnectionMaxLength} characters");

                RuleFor(x => x.SubmissionRepositoryUrl!)
                    .Must(ValidationConstants.UserProjectAssignment.IsValidSubmissionRepositoryUrl)
                    .WithMessage("SubmissionRepositoryUrl must be a valid GitHub username or a full https://github.com/&lt;username&gt; URL.");
            });

            RuleFor(x => x.SubmissionNotes)
                .MaximumLength(ValidationConstants.UserProjectAssignment.SubmissionNotesMaxLength)
                .WithMessage($"CompletionNotes cannot exceed {ValidationConstants.UserProjectAssignment.SubmissionNotesMaxLength} characters");
        }
    }
}