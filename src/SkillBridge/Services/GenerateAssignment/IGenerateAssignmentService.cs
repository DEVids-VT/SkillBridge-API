using SkillBridge.Models.Request;

namespace SkillBridge.Services.GenerateAssignment
{
    public interface IGenerateAssignmentService
    {
        Task<Models.Entities.ProjectAssignment> GenerateAssignmentAsync(CandidateRequirementsRequest candidate);
    }
}
