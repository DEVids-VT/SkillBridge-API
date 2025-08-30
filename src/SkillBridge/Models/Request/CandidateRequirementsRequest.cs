using SkillBridge.Models.Enums;

namespace SkillBridge.Models.Request
{
    public class CandidateRequirementsRequest
    {
        public string PositionTitle { get; set; } = string.Empty;
        public string DepartmentOrArea { get; set; } = string.Empty;
        public string CompanyIndustry { get; set; } = string.Empty;
        public ExperienceLevel ExperienceLevel { get; set; }
        public int MinExperienceYears { get; set; } = 0;
        public int? MaxExperienceYears { get; set; }
        public EducationLevel MinEducationLevel { get; set; }
        public List<string> PreferredEducationFields { get; set; } = new();
        public List<string> RequiredCertifications { get; set; } = new();
        public List<string> PreferredCertifications { get; set; } = new();
        public List<CompetencyRequirement> RequiredCompetencies { get; set; } = new();
        public List<CompetencyRequirement> PreferredCompetencies { get; set; } = new();
        public List<WorkCondition> WorkConditions { get; set; } = new();
        public List<string> LanguageRequirements { get; set; } = new();
        public string PositionSummary { get; set; } = string.Empty;
        public string IdealCandidateProfile { get; set; } = string.Empty;
        public List<string> KeyResponsibilities { get; set; } = new();
        public List<PersonalityTrait> DesiredPersonalityTraits { get; set; } = new();
        public string CultureFitDescription { get; set; } = string.Empty;
        public Dictionary<string, object> CustomCriteria { get; set; } = new();
    }

    public class CompetencyRequirement
    {
        public string Name { get; set; } = string.Empty;
        public CompetencyType Type { get; set; }
        public ProficiencyLevel RequiredLevel { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsMandatory { get; set; } = true;
    }

    public class WorkCondition
    {
        public string Type { get; set; } = string.Empty; // Remote, Travel, Shift, Physical
        public string Description { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = true;
    }

    public class PersonalityTrait
    {
        public string TraitName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ImportanceLevel Importance { get; set; }
    }
}
