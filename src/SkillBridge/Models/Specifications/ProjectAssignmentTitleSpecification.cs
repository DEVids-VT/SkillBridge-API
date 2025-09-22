using Auth0.ManagementApi.Models;
using SkillBridge.Models.Entities;
using System.Linq.Expressions;

namespace SkillBridge.Models.Specifications
{
    public class ProjectAssignmentTitleSpecification : Specification<ProjectAssignment>
    {
        private readonly string? title;

        public ProjectAssignmentTitleSpecification(string? name) => this.title = name;

        protected override bool Include => !string.IsNullOrWhiteSpace(title);

        public override Expression<Func<ProjectAssignment, bool>> ToExpression()
            => ex => ex.Title.ToLower().Contains(title!.ToLower());
    }
}
