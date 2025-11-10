using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTested.AspNetCore.Mvc;
using SkillBridge.Controllers;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.GenerateAssignment; // <-- if your namespace is different, adjust this
using Xunit;

namespace SkillBridge.UnitTests.Controllers.GenerateAssignmentControllerTests
{
    public class GenerateAssignmentControllerTests
    {
        private static Mock<IGenerateAssignmentService> ServiceMock(MockBehavior behavior = MockBehavior.Strict)
            => new(behavior);

        private static ProjectAssignmentResponse SampleAssignment(Guid? id = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            Title = "Generated assignment",
            CompanyName = "ACME"
            // add more props here if your DTO requires them
        };

        // ─────────────────────────────────────────────────────────────
        // Controller-level route
        // ─────────────────────────────────────────────────────────────
        [Fact]
        public void Controller_Should_Have_Generate_Route_Prefix()
        {
            var svc = ServiceMock(MockBehavior.Loose).Object;

            MyController<GenerateAssignmentController>
                .Instance(i => i.WithDependencies(svc))
                .ShouldHave()
                .Attributes(a => a.SpecifyingRoute("api/g"));
        }

        // ─────────────────────────────────────────────────────────────
        // Generate() attributes: [HttpPost("{companyId}")] + [Authorize(Policy="Company")]
        // ─────────────────────────────────────────────────────────────
        [Fact]
        public void Generate_Should_Have_Post_And_Company_Policy()
        {
            var companyId = Guid.NewGuid();
            var req = new CandidateRequirementsRequest(); // fill fields if needed by your code

            var svc = ServiceMock();
            svc.Setup(s => s.GenerateAssignmentAsync(companyId, req))
               .Returns(Task.FromResult(SampleAssignment()));

            MyController<GenerateAssignmentController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Generate(companyId, req))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("POST")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            // Assert policy via reflection
            var mi = typeof(GenerateAssignmentController)
                .GetMethod(nameof(GenerateAssignmentController.Generate),
                           new[] { typeof(Guid), typeof(CandidateRequirementsRequest) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("Company", au!.Policy);
        }

        // ─────────────────────────────────────────────────────────────
        // Generate() behavior: returns CreatedAtAction pointing to
        // ProjectAssignmentsController.GetById with correct route values
        // ─────────────────────────────────────────────────────────────
        [Fact]
        public void Generate_Should_Return_CreatedAtAction_To_ProjectAssignments_GetById()
        {
            var companyId = Guid.NewGuid();
            var req = new CandidateRequirementsRequest(); // add details if your service needs them

            var saved = SampleAssignment();

            var svc = ServiceMock();
            svc.Setup(s => s.GenerateAssignmentAsync(companyId, req))
               .Returns(Task.FromResult(saved));

            MyController<GenerateAssignmentController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Generate(companyId, req))
                .ShouldReturn()
                .Created(cr => cr
                    .Passing(result =>
                    {
                        var created = Assert.IsType<CreatedAtActionResult>(result);

                        // Action/controller
                        Assert.Equal(nameof(ProjectAssignmentsController.GetById), created.ActionName);
                        Assert.Equal("ProjectAssignments", created.ControllerName);

                        // Route values
                        Assert.NotNull(created.RouteValues);
                        Assert.Equal(saved.Id, created.RouteValues["id"]);

                        // Body
                        Assert.Same(saved, created.Value);
                    }));

            svc.VerifyAll();
        }
    }
}
