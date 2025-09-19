namespace SkillBridge.Infrastructure.Configuration
{
    /// <summary>
    /// Represents OpenAI configuration settings for making API requests.
    /// </summary>
    public class OpenAISettings
    {
        /// <summary>
        /// The API key used to authenticate with the OpenAI service.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// The deployment model name to be used (e.g., "gpt-4o").
        /// </summary>
        public string Model { get; set; } = "gpt-5-mini-2025-08-07";
    }
}
