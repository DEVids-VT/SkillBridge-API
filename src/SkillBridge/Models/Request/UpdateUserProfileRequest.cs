namespace SkillBridge.Models.Request
{
    public class UpdateUserProfileRequest
    {
        public IFormFile? ProfilePicture { get; set; } // Will also mirror to Auth0 "picture"

        // App-owned (your DB)
        public IFormFile? CVUpload { get; set; }
        public string? GitHubConnection { get; set; }
    }
}
