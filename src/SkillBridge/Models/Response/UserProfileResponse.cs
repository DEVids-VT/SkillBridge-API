namespace SkillBridge.Models.Response
{
    public class UserProfileResponse
    {
        //public string FullName { get; set; } = string.Empty;
        //public string Email { get; set; } = string.Empty;
        //public string Username { get; set; } = string.Empty;    
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string ProfilePicture { get; set; } = string.Empty;

        public string CVUpload { get; set; } = string.Empty;
        public string GitHubConnection { get; set; } = string.Empty;
    }
}
