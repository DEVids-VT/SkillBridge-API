using NJsonSchema;
using NJsonSchema.Generation;
using Stripe.Apps;
using System;
using System.Text;

namespace SkillBridge.Infrastructure.Ai
{
    /// <summary>
    /// Represents a constructed prompt containing a system instruction and data
    /// </summary>
    public class Prompt<TResponse> where TResponse : class
    {
        /// <summary>
        /// Gets the system instruction that provides context for the AI
        /// </summary>
        public string SystemPrompt { get; }
        
        /// <summary>
        /// Gets the content that will be processed using the system prompt
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Prompt"/> class
        /// </summary>
        /// <param name="systemPrompt">The system instruction for the AI</param>
        /// <param name="content">The content to be processed</param>
        public Prompt(string systemPrompt, string content)
        {
            SystemPrompt = systemPrompt ?? throw new ArgumentNullException(nameof(systemPrompt));
            Content = content ?? throw new ArgumentNullException(nameof(content));

            var response = JsonSchema.FromType<TResponse>().ToJson();
            var sb = new StringBuilder(SystemPrompt);
            sb.AppendLine("## OUTPUT FORMAT");
            sb.AppendLine(response);
            SystemPrompt = sb.ToString();
        }

    }
}