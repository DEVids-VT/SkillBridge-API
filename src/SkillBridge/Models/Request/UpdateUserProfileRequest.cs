namespace SkillBridge.Models.Request
{
    public class UpdateUserProfileRequest
    {
        public string? Username { get; set; }   // Auth0 "user_name" (DB connections only)
        public string? FullName { get; set; }   // Auth0 "name"
        public string? ProfilePicture { get; set; } // Will also mirror to Auth0 "picture"

        // App-owned (your DB)
        public string? CVUpload { get; set; }
        public string? GitHubConnection { get; set; }
    }
}
