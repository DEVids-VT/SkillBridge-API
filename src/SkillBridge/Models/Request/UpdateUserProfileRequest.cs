namespace SkillBridge.Models.Request
{
    public class UpdateUserProfileRequest
    {
        public string Interests { get; set; } = string.Empty;

        public string CVUpload { get; set; } = string.Empty;

        public string GitHubConnection { get; set; } = string.Empty;
    }
}
