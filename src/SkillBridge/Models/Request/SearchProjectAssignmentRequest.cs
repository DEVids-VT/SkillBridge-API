using SkillBridge.Models.Enums;

namespace SkillBridge.Models.Request
{
    public class SearchProjectAssignmentRequest
    {
        public string? Title { get; set; }
        public ProjectAssignmentLevel? Level { get; set; }
        public DateTime? DeadlineAfter { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanySector { get; set; }
        public ICollection<string> ProjectSkills { get; set; } = new List<string>();
    }
}
