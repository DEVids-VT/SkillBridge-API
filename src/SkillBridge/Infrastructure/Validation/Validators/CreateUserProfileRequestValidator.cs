using FluentValidation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators
{
    /// <summary>
    /// Validator for creating user profiles.
    /// </summary>
    public class CreateUserProfileRequestValidator : AbstractValidator<CreateUserProfileRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserProfileRequestValidator"/> class.
        /// </summary>
        public CreateUserProfileRequestValidator()
        {
            // Profile picture (optional)
            When(x => x.ProfilePicture != null, () =>
            {
                RuleFor(x => x.ProfilePicture!)
                    .Must(f => f.Length <= ValidationConstants.UserProfile.ProfilePictureMaxBytes)
                    .WithMessage($"Profile picture must be ≤ {ValidationConstants.UserProfile.BytesToMb(ValidationConstants.UserProfile.ProfilePictureMaxBytes)} MB.");

                RuleFor(x => x.ProfilePicture!)
                    .Must(f => ValidationConstants.UserProfile.AllowedImageTypes.Contains(f.ContentType, StringComparer.OrdinalIgnoreCase))
                    .WithMessage($"Profile picture must be one of: {string.Join(", ", ValidationConstants.UserProfile.AllowedImageTypes)}.");
            });

            // CV (optional)
            When(x => x.CVUpload != null, () =>
            {
                RuleFor(x => x.CVUpload!)
                    .Must(f => f.Length <= ValidationConstants.UserProfile.CvMaxBytes)
                    .WithMessage($"CV must be ≤ {ValidationConstants.UserProfile.BytesToMb(ValidationConstants.UserProfile.CvMaxBytes)} MB.");

                RuleFor(x => x.CVUpload!)
                    .Must(f => ValidationConstants.UserProfile.AllowedCvTypes.Contains(f.ContentType, StringComparer.OrdinalIgnoreCase))
                    .WithMessage($"CV must be one of: {string.Join(", ", ValidationConstants.UserProfile.AllowedCvTypes)}.");
            });

            // GitHubConnection (optional)
            When(x => !string.IsNullOrWhiteSpace(x.GitHubConnection), () =>
            {
                RuleFor(x => x.GitHubConnection!)
                    .MaximumLength(ValidationConstants.UserProfile.GitHubConnectionMaxLength)
                    .WithMessage($"GitHubConnection cannot exceed {ValidationConstants.UserProfile.GitHubConnectionMaxLength} characters");

                RuleFor(x => x.GitHubConnection!)
                    .Must(ValidationConstants.UserProfile.IsValidGitHubConnection)
                    .WithMessage("GitHubConnection must be a valid GitHub username or a full https://github.com/&lt;username&gt; URL.");
            });
        }
    }
}
