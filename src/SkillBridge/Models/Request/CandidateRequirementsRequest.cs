using SkillBridge.Models.Enums;

namespace SkillBridge.Models.Request
{
    public class CandidateRequirementsRequest
    {
        public string PositionTitle { get; set; } = default!;
        public string DepartmentOrArea { get; set; } = default!;
        public string CompanyIndustry { get; set; } = default!;
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
        public string PositionSummary { get; set; } = default!;
        public string IdealCandidateProfile { get; set; } = default!;
        public List<string> KeyResponsibilities { get; set; } = new();
        public List<PersonalityTrait> DesiredPersonalityTraits { get; set; } = new();
        public string CultureFitDescription { get; set; } = default!;
        public Dictionary<string, object> CustomCriteria { get; set; } = new();
    }

    public class CompetencyRequirement
    {
        public string Name { get; set; } = default!;
        public CompetencyType Type { get; set; }
        public ProficiencyLevel RequiredLevel { get; set; }
        public string Description { get; set; } = default!;
        public bool IsMandatory { get; set; } = true;
    }

    public class WorkCondition
    {
        public string Type { get; set; } = default!; // Remote, Travel, Shift, Physical
        public string Description { get; set; } = default!;
        public bool IsRequired { get; set; } = true;
    }

    public class PersonalityTrait
    {
        public string TraitName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public ImportanceLevel Importance { get; set; }
    }
}
