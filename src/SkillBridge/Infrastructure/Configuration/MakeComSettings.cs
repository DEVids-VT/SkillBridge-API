namespace SkillBridge.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration settings for Make.com integration
    /// </summary>
    public class MakeComSettings
    {
        /// <summary>
        /// API key for authenticating with Make.com webhooks
        /// </summary>
        public string ApiKey { get; set; } = default!;

        /// <summary>
        /// Webhook endpoint URL for Make.com scenario
        /// </summary>
        public string Endpoint { get; set; } = default!;
    }
}
