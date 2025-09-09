namespace SkillBridge.Models.Request
{
    /// <summary>
    /// Request model used for creating a new user profile.
    /// Contains optional fields for profile picture, CV upload, and GitHub connection.
    /// </summary>
    public class CreateUserProfileRequest
    {
        /// <summary>
        /// Gets or sets the profile picture file uploaded by the user.
        /// This file is optional.
        /// </summary>
        public IFormFile? ProfilePicture { get; set; }

        /// <summary>
        /// Gets or sets the CV (Curriculum Vitae) file uploaded by the user.
        /// This file is optional.
        /// </summary>
        public IFormFile? CVUpload { get; set; }

        /// <summary>
        /// Gets or sets the GitHub connection link or username provided by the user.
        /// This field is optional.
        /// </summary>
        public string? GitHubConnection { get; set; }
    }
}
