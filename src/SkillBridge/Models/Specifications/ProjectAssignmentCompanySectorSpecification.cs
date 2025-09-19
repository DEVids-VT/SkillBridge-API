using SkillBridge.Models.Entities;
using System.Linq.Expressions;

namespace SkillBridge.Models.Specifications
{
    public class ProjectAssignmentCompanySectorSpecification : Specification<ProjectAssignment>
    {
        private readonly string? companySector;

        public ProjectAssignmentCompanySectorSpecification(string? companySector) => this.companySector = companySector;

        protected override bool Include => companySector != null;

        public override Expression<Func<ProjectAssignment, bool>> ToExpression()
            => ex => ex.Company != null && ex.Company.Sector == companySector;
    }
}
