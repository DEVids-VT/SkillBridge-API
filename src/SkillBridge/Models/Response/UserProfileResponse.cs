namespace SkillBridge.Models.Response
{
    public class UserProfileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;

        public string CVUpload { get; set; } = string.Empty;
        public string GitHubConnection { get; set; } = string.Empty;
    }
}
