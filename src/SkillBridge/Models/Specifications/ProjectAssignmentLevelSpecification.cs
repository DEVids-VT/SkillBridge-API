using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using System.Linq.Expressions;

namespace SkillBridge.Models.Specifications
{
    public class ProjectAssignmentLevelSpecification : Specification<ProjectAssignment>
    {
        private readonly ProjectAssignmentLevel? level;

        public ProjectAssignmentLevelSpecification(ProjectAssignmentLevel? level) => this.level = level;

        protected override bool Include => level != null;

        public override Expression<Func<ProjectAssignment, bool>> ToExpression()
            => ex => ex.Level == level;
    }
}
