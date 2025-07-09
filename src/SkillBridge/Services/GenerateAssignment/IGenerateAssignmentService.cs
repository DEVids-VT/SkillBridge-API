using SkillBridge.Models.Request;
using SkillBridge.Models.Response;

namespace SkillBridge.Services.GenerateAssignment
{
    public interface IGenerateAssignmentService
    {
        /// <summary>
        /// Generates and saves a project assignment for a company
        /// </summary>
        /// <param name="companyId">The ID of the company creating the assignment</param>
        /// <param name="candidate">The candidate requirements</param>
        /// <returns>The saved project assignment response</returns>
        Task<ProjectAssignmentResponse> GenerateAssignmentAsync(Guid companyId, CandidateRequirementsRequest candidate);
    }
}
