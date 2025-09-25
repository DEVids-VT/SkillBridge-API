using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Infrastructure.Ai
{
    public interface IAgentClient
    {
        /// <summary>
        /// Generates assignment using an agent
        /// </summary>
        /// <returns>The generated assignment</returns>
        Task<ProjectAssignmentResponse> GenerateAssignment(Guid companyId, CandidateRequirementsRequest candidate);
    }
}
