namespace SkillBridge.Models.Response
{
    public class UserProfileResponse
    {
        public string Id { get; set; } = default!;
        public string? ProfilePicture { get; set; } 

        public string? CVUpload { get; set; } 
        public string? GitHubConnection { get; set; } 
    }
}
