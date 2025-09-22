using SkillBridge.Models.Entities;
using SkillBridge.Models.Enums;
using System.Linq.Expressions;

namespace SkillBridge.Models.Specifications
{
    public class ProjectAssignmentCompanyNameSpecification : Specification<ProjectAssignment>
    {
        private readonly string? companyName;

        public ProjectAssignmentCompanyNameSpecification(string? companyName) => this.companyName = companyName;

        protected override bool Include => companyName != null;

        public override Expression<Func<ProjectAssignment, bool>> ToExpression()
            => ex => ex.Company != null && ex.Company.Name == companyName;
    }
}
