namespace SkillBridge.Models.Request
{
    public class CandidateRequirementsRequest
    {
        public string RoleTitle { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string SeniorityLevel { get; set; } = string.Empty;
        public List<string> RequiredSkills { get; set; } = new();
        public List<string> RelevantTechnologies { get; set; } = new();
        public List<string>? IndustryExperience { get; set; }
        public string DescriptionOfIdealCandidate { get; set; } = string.Empty;
    }
}
