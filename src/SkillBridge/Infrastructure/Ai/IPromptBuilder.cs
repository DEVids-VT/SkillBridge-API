using System.Collections.Generic;
using Auth0.ManagementApi.Models.Prompts;

namespace SkillBridge.Infrastructure.Ai
{
    /// <summary>
    /// Interface for building prompts that combine system instructions with data models
    /// </summary>
    public interface IPromptBuilder
    {
        /// <summary>
        /// Builds a prompt using a system prompt from a markdown file and data model formatted as JSON
        /// </summary>
        /// <typeparam name="T">The type of the data model</typeparam>
        /// <param name="promptFileName">The name of the markdown file containing the system prompt</param>
        /// <param name="model">The data model to include in the prompt</param>
        /// <returns>A constructed prompt</returns>
        Prompt<T> BuildFromFile<T>(string promptFileName, object model) where T : class;

        /// <summary>
        /// Builds a prompt with a system prompt from a markdown file and multiple data models combined, all formatted as JSON
        /// </summary>
        /// <typeparam name="T">The type of the data models</typeparam>
        /// <param name="promptFileName">The name of the markdown file containing the system prompt</param>
        /// <param name="models">Collection of models with their names to include in the prompt</param>
        /// <returns>A constructed prompt</returns>
        Prompt<T> BuildFromFile<T>(string promptFileName, IEnumerable<object> models) where T : class;


        /// <summary>
        /// Builds a prompt with the given system prompt and data model formatted as JSON
        /// </summary>
        /// <typeparam name="T">The type of the data model</typeparam>
        /// <param name="systemPrompt">The system instruction for the AI</param>
        /// <param name="model">The data model to include in the prompt</param>
        /// <returns>A constructed prompt</returns>
        Prompt<T> Build<T>(string systemPrompt, object model) where T : class;

        /// <summary>
        /// Builds a prompt with the given system prompt and multiple data models combined, all formatted as JSON
        /// </summary>
        /// <typeparam name="T">The type of the data models</typeparam>
        /// <param name="systemPrompt">The system instruction for the AI</param>
        /// <param name="models">Collection of models with their names to include in the prompt</param>
        /// <returns>A constructed prompt</returns>
        Prompt<T> Build<T>(string systemPrompt, IEnumerable<object> models) where T : class;
    }
}