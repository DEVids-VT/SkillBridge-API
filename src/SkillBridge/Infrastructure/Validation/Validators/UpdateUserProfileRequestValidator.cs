using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using SkillBridge.Infrastructure.Validation;
using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Validation.Validators
{
    /// <summary>
    /// Validator for both creating and updating user profiles.
    /// </summary>
    public class UserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
    {
        private static readonly string[] AllowedImageContentTypes =
        {
            "image/jpeg", "image/png", "image/webp", "image/gif"
        };

        private static readonly string[] AllowedCvContentTypes =
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "text/plain"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileRequestValidator"/> class.
        /// </summary>
        public UserProfileRequestValidator()
        {
            // Profile picture (optional)
            When(x => x.ProfilePicture != null, () =>
            {
                RuleFor(x => x.ProfilePicture!)
                    .Must(f => f.Length <= ValidationConstants.UserProfile.ProfilePictureMaxBytes)
                    .WithMessage($"Profile picture must be ≤ {BytesToMb(ValidationConstants.UserProfile.ProfilePictureMaxBytes)} MB.");

                RuleFor(x => x.ProfilePicture!)
                    .Must(f => AllowedImageContentTypes.Contains(f.ContentType, StringComparer.OrdinalIgnoreCase))
                    .WithMessage($"Profile picture must be one of: {string.Join(", ", AllowedImageContentTypes)}.");
            });

            // CV (optional)
            When(x => x.CVUpload != null, () =>
            {
                RuleFor(x => x.CVUpload!)
                    .Must(f => f.Length <= ValidationConstants.UserProfile.CvMaxBytes)
                    .WithMessage($"CV must be ≤ {BytesToMb(ValidationConstants.UserProfile.CvMaxBytes)} MB.");

                RuleFor(x => x.CVUpload!)
                    .Must(f => AllowedCvContentTypes.Contains(f.ContentType, StringComparer.OrdinalIgnoreCase))
                    .WithMessage($"CV must be one of: {string.Join(", ", AllowedCvContentTypes)}.");
            });

            // GitHubConnection (optional)
            When(x => !string.IsNullOrWhiteSpace(x.GitHubConnection), () =>
            {
                RuleFor(x => x.GitHubConnection!)
                    .MaximumLength(ValidationConstants.UserProfile.GitHubConnectionMaxLength)
                    .WithMessage($"GitHubConnection cannot exceed {ValidationConstants.UserProfile.GitHubConnectionMaxLength} characters");

                RuleFor(x => x.GitHubConnection!)
                    .Must(IsValidGitHubConnection)
                    .WithMessage("GitHubConnection must be a valid GitHub username or a full https://github.com/&lt;username&gt; URL.");
            });

            
            
        }

        private static bool IsValidGitHubConnection(string value)
        {
            var trimmed = value.Trim();

            if (IsValidGitHubUsername(trimmed))
                return true;

            if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp)
                && string.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase))
            {
                var username = uri.AbsolutePath
                    .Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();

                return !string.IsNullOrEmpty(username) && IsValidGitHubUsername(username);
            }

            return false;
        }

        private static bool IsValidGitHubUsername(string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate)) return false;
            if (candidate.Length < 1 || candidate.Length > 39) return false;
            if (candidate.StartsWith("-") || candidate.EndsWith("-")) return false;
            if (candidate.Contains("--")) return false;

            return candidate.All(ch => char.IsLetterOrDigit(ch) || ch == '-');
        }

        private static long BytesToMb(long bytes) =>
            (bytes + (1024 * 1024 - 1)) / (1024 * 1024); // ceil to MB
    }
}
