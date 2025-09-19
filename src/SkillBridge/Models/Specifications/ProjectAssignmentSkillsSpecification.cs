using SkillBridge.Models.Entities;
using System.Linq.Expressions;

namespace SkillBridge.Models.Specifications
{
    public class ProjectAssignmentSkillsSpecification : Specification<ProjectAssignment>
    {
        private readonly ICollection<string> skills;

        public ProjectAssignmentSkillsSpecification(ICollection<string> skills) => this.skills = skills;

        protected override bool Include => skills != null && skills.Count != 0;

        public override Expression<Func<ProjectAssignment, bool>> ToExpression()
            => ex => ex.ProjectSkills != null && ex.ProjectSkills.Count != 0 && ex.ProjectSkills
                .Where(x => x.Skill != null)
                .Select(x => x.Skill!.Name)
                .Any(skill => skills.Contains(skill));
    }
}
