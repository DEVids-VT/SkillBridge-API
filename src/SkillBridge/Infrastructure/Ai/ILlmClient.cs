namespace SkillBridge.Infrastructure.Ai
{
    public interface ILlmClient
    {
        
        /// <summary>
        /// Generates a response using a prompt model
        /// </summary>
        /// <typeparam name="TResponse">The expected response type</typeparam>
        /// <param name="promptModel">The prompt model containing the system prompt and data</param>
        /// <returns>The generated response</returns>
        Task<TResponse> GenerateAsync<TResponse>(Prompt<TResponse> promptModel) where TResponse : class;
    }
}
