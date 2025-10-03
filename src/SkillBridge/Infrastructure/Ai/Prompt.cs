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
        //public Prompt(string systemPrompt, string content)
        //{
        //    SystemPrompt = systemPrompt ?? throw new ArgumentNullException(nameof(systemPrompt));
        //    Content = content ?? throw new ArgumentNullException(nameof(content));

        //    var response = JsonSchema.FromType<TResponse>().ToJson();
        //    var sb = new StringBuilder(SystemPrompt);
        //    sb.AppendLine("## OUTPUT FORMAT");
        //    sb.AppendLine(response);
        //    SystemPrompt = sb.ToString();
        //}
        public Prompt(string systemPrompt, string content)
        {
            SystemPrompt = systemPrompt ?? throw new ArgumentNullException(nameof(systemPrompt));
            Content = content ?? throw new ArgumentNullException(nameof(content));

            var settings = new SystemTextJsonSchemaGeneratorSettings();
            settings.SchemaProcessors.Add(new TimeSpanSchemaProcessor());

            var generator = new JsonSchemaGenerator(settings);
            var schema = generator.Generate(typeof(TResponse));
            var response = schema.ToJson();

            var sb = new StringBuilder(SystemPrompt);
            sb.AppendLine("## OUTPUT FORMAT");
            sb.AppendLine(response);
            SystemPrompt = sb.ToString();
        }
    }
    public sealed class TimeSpanSchemaProcessor : ISchemaProcessor
    {
        public void Process(SchemaProcessorContext context)
        {
            // Prefer ContextualType over the obsolete Type
            var ctx = context.ContextualType;
            if (ctx == null) return;

            // Get the underlying CLR type and nullability
            var clrType = ctx.Type;                              // e.g., typeof(TimeSpan) or typeof(Nullable<TimeSpan>)
            var isNullable = ctx.IsNullableType;                 // true for TimeSpan?

            // Unwrap Nullable<T> if present
            var underlying = isNullable && Nullable.GetUnderlyingType(clrType) != null
                ? Nullable.GetUnderlyingType(clrType)!
                : clrType;

            if (underlying == typeof(TimeSpan))
            {
                // Treat TimeSpan as a primitive string in "c" format: d.hh:mm:ss
                context.Schema.Reference = null;                 // don't recurse into TimeSpan members
                context.Schema.Type = JsonObjectType.String;
                context.Schema.Format = "time-span-dotnet";
                context.Schema.Description = "Duration in .NET TimeSpan \"c\" format: d.hh:mm:ss (e.g., 14.00:00:00).";
                context.Schema.Pattern = @"^\d+\.\d{2}:\d{2}:\d{2}$";
                context.Schema.Example = "14.00:00:00";

                // Preserve nullability for TimeSpan?
                context.Schema.IsNullableRaw = isNullable;
            }
        }
    }

}