namespace SkillBridge.Models.Request
{
    public class UpdateCompanyLogoRequest
    {
        /// <summary>
        /// Gets or sets the URL to the company logo
        /// </summary>
        public IFormFile? LogoUrl { get; set; }
    }
}
