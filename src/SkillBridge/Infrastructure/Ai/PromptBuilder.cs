using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SkillBridge.Infrastructure.Ai
{
    /// <summary>
    /// Builds prompts that combine system instructions with data models
    /// </summary>
    public class PromptBuilder : IPromptBuilder
    {
        private readonly string _promptsDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptBuilder"/> class
        /// </summary>
        public PromptBuilder()
        {
            var projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)
                ?.Parent?.Parent?.Parent?.FullName;

            _promptsDirectory = Path.Combine(projectRoot ?? "", "Infrastructure", "Prompts", "Files");

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptBuilder"/> class with a specific prompts directory
        /// </summary>
        /// <param name="promptsDirectory">The directory containing prompt markdown files</param>
        public PromptBuilder(string promptsDirectory)
        {
            _promptsDirectory = promptsDirectory ?? throw new ArgumentNullException(nameof(promptsDirectory));
            
            if (!Directory.Exists(_promptsDirectory))
                throw new DirectoryNotFoundException($"The prompts directory '{_promptsDirectory}' does not exist.");
        }

        ///<inheritdoc/>
        public Prompt<T> BuildFromFile<T>(string promptFileName, object model) where T : class
        {
            var systemPrompt = ReadSystemPromptFromFile(promptFileName);
            return Build<T>(systemPrompt, model);
        }
        ///<inheritdoc/>

        public Prompt<T> BuildFromFile<T>(string promptFileName, IEnumerable<object> models) where T : class
        {
            var systemPrompt = ReadSystemPromptFromFile(promptFileName);
            return Build<T>(systemPrompt, models);
        }

        ///<inheritdoc/>

        public Prompt<T> Build<T>(string systemPrompt, object model) where T : class
        {
            if (string.IsNullOrEmpty(systemPrompt))
                throw new ArgumentException("System prompt cannot be null or empty", nameof(systemPrompt));
            
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            string content = JsonConvert.SerializeObject(model, Formatting.Indented);
            return new Prompt<T>(systemPrompt, content);
        }

        ///<inheritdoc/>

        public Prompt<T> Build<T>(string systemPrompt, IEnumerable<object> models) where T : class
        {
            if (string.IsNullOrEmpty(systemPrompt))
                throw new ArgumentException("System prompt cannot be null or empty", nameof(systemPrompt));

            if (models == null || !models.Any())
                throw new ArgumentException("At least one model must be provided", nameof(models));

            var contentBuilder = new StringBuilder();

            foreach (var model in models)
            {
                if (model != null)
                {
                    contentBuilder.AppendLine(JsonConvert.SerializeObject(model, Formatting.Indented));
                }

                contentBuilder.AppendLine();
            }

            return new Prompt<T>(systemPrompt, contentBuilder.ToString().TrimEnd());
        }

        #region Helpers

        /// <summary>
        /// Reads a system prompt from a markdown file
        /// </summary>
        /// <param name="fileName">The name of the markdown file (with or without .md extension)</param>
        /// <returns>The content of the markdown file as a string</returns>
        private string ReadSystemPromptFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

            // Ensure the file has .md extension
            if (!fileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                fileName += ".md";

            string filePath = Path.Combine(_promptsDirectory, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"System prompt file not found: {filePath}", filePath);

            return File.ReadAllText(filePath);
        }

        #endregion
    }
}