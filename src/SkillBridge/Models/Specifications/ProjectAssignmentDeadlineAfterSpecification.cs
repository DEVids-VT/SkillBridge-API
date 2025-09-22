using SkillBridge.Models.Entities;
using System.Linq.Expressions;

namespace SkillBridge.Models.Specifications
{
    public class ProjectAssignmentDeadlineAfterSpecification : Specification<ProjectAssignment>
    {
        private readonly DateTime? deadLineAfter;

        public ProjectAssignmentDeadlineAfterSpecification(DateTime? deadLineAfter) => this.deadLineAfter = deadLineAfter;

        protected override bool Include => deadLineAfter != null;

        public override Expression<Func<ProjectAssignment, bool>> ToExpression()
            => ex => ex.Deadline > deadLineAfter;
    }
}
