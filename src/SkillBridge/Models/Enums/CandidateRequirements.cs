namespace SkillBridge.Models.Enums
{
    public enum ExperienceLevel
    {
        EntryLevel,
        Junior,
        MidLevel,
        Senior,
        Lead,
        Executive,
        Intern
    }

    public enum EducationLevel
    {
        None,
        HighSchool,
        Vocational,
        AssociateDegree,
        BachelorsDegree,
        MastersDegree,
        Doctorate,
        Professional
    }

    public enum CompetencyType
    {
        Technical,          // Техническо умение
        Functional,         // Функционално умение
        Leadership,         // Лидерски умения
        Communication,      // Комуникационни умения
        Analytical,         // Аналитични умения
        Creative,           // Творчески умения
        ProjectManagement,  // Управление на проекти
        CustomerService,    // Обслужване на клиенти
        Financial,          // Финансови умения
        Regulatory,         // Регулаторни познания
        Industry,           // Индустриални познания
        Language,           // Езикови умения
        Software,           // Софтуерни умения
        Hardware,           // Хардуерни умения
        Process,            // Процесни умения
        Safety,             // Безопасност
        Quality,            // Качество
        Other               // Други
    }

    public enum ProficiencyLevel
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert,
        Master
    }

    public enum ImportanceLevel
    {
        Low,
        Medium,
        High,
        Critical
    }

}
