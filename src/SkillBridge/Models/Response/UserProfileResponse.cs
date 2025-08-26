namespace SkillBridge.Models.Response
{
    public class UserProfileResponse
    {

        /// <summary>
        /// Gets or sets the date when this user profile was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date when this user profile was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        public string Interests { get; set; } = string.Empty;

        public string CVUpload { get; set; } = string.Empty;

        public string GitHubConnection { get; set; } = string.Empty;
    }
}
