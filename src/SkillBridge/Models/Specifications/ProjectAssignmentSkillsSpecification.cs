using SkillBridge.Models.Entities;
using System.Linq.Expressions;

namespace SkillBridge.Models.Specifications
{
    public class ProjectAssignmentSkillsSpecification : Specification<ProjectAssignment>
    {
        private readonly string[] skills;

        public ProjectAssignmentSkillsSpecification(ICollection<string> skills)
            => this.skills = skills?.ToArray() ?? Array.Empty<string>();

        protected override bool Include => skills.Length > 0;

        public override Expression<Func<ProjectAssignment, bool>> ToExpression()
            => ex => ex.ProjectSkills.Any(ps => ps.Skill != null && skills.Contains(ps.Skill.Name));

    }
}
