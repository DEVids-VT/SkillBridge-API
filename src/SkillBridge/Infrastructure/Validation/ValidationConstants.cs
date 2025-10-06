using System;

namespace SkillBridge.Infrastructure.Validation;

/// <summary>
/// Contains constants for validation rules across the application.
/// These values are extracted from entity configurations to ensure consistency.
/// </summary>
public static class ValidationConstants
{
    /// <summary>
    /// Constants related to the Skill entity
    /// </summary>
    public static class Skill
    {
        /// <summary>
        /// Maximum length for the Name property
        /// </summary>
        public const int NameMaxLength = 100;
        
        /// <summary>
        /// Maximum length for the Description property
        /// </summary>
        public const int DescriptionMaxLength = 500;
    }
    
    /// <summary>
    /// Constants related to the ProjectAssignment entity
    /// </summary>
    public static class ProjectAssignment
    {
        /// <summary>
        /// Maximum length for the Title property
        /// </summary>
        public const int TitleMaxLength = 200;
        
        /// <summary>
        /// Maximum length for the Description property
        /// </summary>
        public const int DescriptionMaxLength = 20000;
        
        /// <summary>
        /// Maximum length for the Summary property
        /// </summary>
        public const int SummaryMaxLength = 500;
        
        /// <summary>
        /// Maximum length for the LearningBenefits property
        /// </summary>
        public const int LearningBenefitsMaxLength = 1000;
        
        /// <summary>
        /// Maximum length for the SuggestedApproach property
        /// </summary>
        public const int SuggestedApproachMaxLength = 1000;
    }
    
    /// <summary>
    /// Constants related to the Company entity
    /// </summary>
    public static class Company
    {
        /// <summary>
        /// Maximum length for the Name property
        /// </summary>
        public const int NameMaxLength = 100;
        
        /// <summary>
        /// Maximum length for the About property
        /// </summary>
        public const int AboutMaxLength = 2000;
        
        /// <summary>
        /// Maximum length for the LogoUrl property
        /// </summary>
        //public const int LogoUrlMaxLength = 500;
        
        /// <summary>
        /// Maximum length for the BannerUrl property
        /// </summary>
        public const int BannerUrlMaxLength = 500;
        
        /// <summary>
        /// Maximum length for the Activities property
        /// </summary>
        public const int ActivitiesMaxLength = 500;
        
        /// <summary>
        /// Maximum length for the Sector property
        /// </summary>
        public const int SectorMaxLength = 100;
        
        /// <summary>
        /// Maximum length for the HeadOfficeLocation property
        /// </summary>
        public const int HeadOfficeLocationMaxLength = 200;
        
        /// <summary>
        /// Maximum length for the Technologies property
        /// </summary>
        public const int TechnologiesMaxLength = 1000;
        
        /// <summary>
        /// Maximum length for the BulgarianOfficeLocations property
        /// </summary>
        public const int BulgarianOfficeLocationsMaxLength = 500;
        
        /// <summary>
        /// Maximum length for the WhyWorkWithUs property
        /// </summary>
        public const int WhyWorkWithUsMaxLength = 2000;
        
        /// <summary>
        /// Maximum length for the WebsiteUrl property
        /// </summary>
        public const int WebsiteUrlMaxLength = 500;
        
        /// <summary>
        /// Maximum length for the ContactName property
        /// </summary>
        public const int ContactNameMaxLength = 100;
        
        /// <summary>
        /// Maximum length for the ContactEmail property
        /// </summary>
        public const int ContactEmailMaxLength = 255;
        
        /// <summary>
        /// Maximum length for the ContactPhone property
        /// </summary>
        public const int ContactPhoneMaxLength = 20;
        
        /// <summary>
        /// Maximum length for the Auth0UserId property
        /// </summary>
        public const int Auth0UserIdMaxLength = 50;
        
        /// <summary>
        /// Minimum valid year for YearEstablished
        /// </summary>
        public const int YearEstablishedMinimum = 1800;

        public const long CompanyLogoMaxBytes = 5L * 1024 * 1024;  // 5 MB

        public static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png" };
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
    }
    
    /// <summary>
    /// Constants related to the ProjectSkill entity
    /// </summary>
    public static class ProjectSkill
    {
        // No specific validation constants required for this entity
    }
    
    /// <summary>
    /// Constants related to the AssignmentTask entity
    /// </summary>
    public static class AssignmentTask
    {
        /// <summary>
        /// Maximum length for the Title property
        /// </summary>
        public const int TitleMaxLength = 200;
        
        /// <summary>
        /// Maximum length for the Description property
        /// </summary>
        public const int DescriptionMaxLength = 2000;
    }
    public static class UserProfile
    {
        public const long ProfilePictureMaxBytes = 5L * 1024 * 1024;  // 5 MB
        public const long CvMaxBytes = 10L * 1024 * 1024; // 10 MB
        public const int GitHubConnectionMaxLength = 200;           // room for full URL

        public static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png" };
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        public static readonly string[] AllowedCvTypes = { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
        public static readonly string[] AllowedCvExtensions = { ".pdf", ".doc", ".docx" };

        public static bool IsValidGitHubConnection(string value)
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

        public static bool IsValidGitHubUsername(string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate)) return false;
            if (candidate.Length < 1 || candidate.Length > 39) return false;
            if (candidate.StartsWith("-") || candidate.EndsWith("-")) return false;
            if (candidate.Contains("--")) return false;

            return candidate.All(ch => char.IsLetterOrDigit(ch) || ch == '-');
        }

        public static long BytesToMb(long bytes) =>
            (bytes + (1024 * 1024 - 1)) / (1024 * 1024); // ceil to MB
    }

    public class UserProjectAssignment
    {
        public const int SubmissionRepositoryUrlMaxLength = UserProfile.GitHubConnectionMaxLength;

        public static bool IsValidSubmissionRepositoryUrl(string value) =>
            UserProfile.IsValidGitHubConnection(value);

        public static int SubmissionNotesMaxLength = 1000;
    }
}