using SkillBridge.Models.Request;

namespace SkillBridge.Infrastructure.Prompts
{
    public static class AssignmentGenerationPrompts
    {
        public static string GenerateAssignmentPrompt(CandidateRequirementsRequest candidate)
        {
            return $$"""
            You are a senior technical interviewer tasked with creating a custom, real-world coding project to assess a candidate's skills and prepare them for a job interview.

            Below is the candidate's profile:

            - Role Title: {{candidate.RoleTitle}}
            - Years of Experience: {{candidate.YearsOfExperience}} years
            - Seniority Level: {{candidate.SeniorityLevel}}
            - Required Skills: {{string.Join(", ", candidate.RequiredSkills)}}
            - Relevant Technologies: {{string.Join(", ", candidate.RelevantTechnologies)}}
            - Industry Experience: {{string.Join(", ", candidate.IndustryExperience ?? new List<string>())}}
            - Description of Ideal Candidate: {{candidate.DescriptionOfIdealCandidate}}

            🔹 Your task:
            Generate a realistic coding project assignment that:
            1. Tests the listed skills and relevant technologies.
            2. Reflects challenges typically faced in the given role.
            3. Matches the seniority and experience level of the candidate.
            4. Optionally incorporates domain knowledge from the listed industry experience.
            5. Helps prepare the candidate for a technical interview.

            🔹 The output must include:
            - `title`: A concise project title.
            - `description`: A brief description of the project.
            - `requirements`: A list of 3–6 key features or tasks to be implemented.
            - `bonusTasks`: 1–3 additional stretch goals or advanced tasks.
            - `evaluationCriteria`: How the project should be reviewed by an interviewer.

            Respond in the following JSON format:
            {
              "title": "string",
              "description": "string",
              "deadline": "yyyy-MM-dd",
              "skills": ["string", "string", "..."],
              "requirements": ["string", "string", "..."],
              "bonusTasks": ["string", "string"],
              "evaluationCriteria": ["string", "string", "..."]
            }
            """;
        }
    }
}
