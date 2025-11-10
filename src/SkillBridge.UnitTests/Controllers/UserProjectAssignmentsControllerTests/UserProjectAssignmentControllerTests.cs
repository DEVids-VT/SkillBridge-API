using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTested.AspNetCore.Mvc;
using SkillBridge.Controllers;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.CurrentUser;
using SkillBridge.Services.UserProjectAssignment;
using Xunit;


namespace SkillBridge.UnitTests.Controllers.UserProjectAssignmentsControllerTests
{
    public class UserProjectAssignmentsControllerTests
    {
        private static Mock<IUserProjectAssignmentService> UserProjectServiceMock(MockBehavior behavior = MockBehavior.Strict)
            => new(behavior);

        private static Mock<ICurrentUser> CurrentUserMock(MockBehavior behavior = MockBehavior.Strict)
            => new(behavior);

        private static UserProjectAssignmentResponse SampleUserProject(bool completed = false)
            => new()
            {
                ProjectAssignment = new ProjectAssignmentResponse
                {
                    Id = Guid.NewGuid(),
                    Title = "Test assignment",
                    CompanyName = "ACME"
                    // add Level/etc if your DTO requires them
                },
                ClaimedAt = DateTime.UtcNow,
                IsCompleted = completed,
                CompletedAt = completed ? DateTime.UtcNow : null
            };

        // ─────────────────────────────────────────────
        // Controller-level attributes
        // ─────────────────────────────────────────────

        [Fact]
        public void Controller_Should_Have_ApiController_Authorize_And_Route()
        {
            var svc = UserProjectServiceMock(MockBehavior.Loose).Object;
            var cu = CurrentUserMock(MockBehavior.Loose).Object;

            MyController<UserProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc, cu))
                .ShouldHave()
                .Attributes(a => a
                    .ContainingAttributeOfType<ApiControllerAttribute>()
                    .ContainingAttributeOfType<AuthorizeAttribute>()
                    .SpecifyingRoute("api/user/projects"));
        }

        // ─────────────────────────────────────────────
        // ClaimProject
        // ─────────────────────────────────────────────

        [Fact]
        public void ClaimProject_Should_Have_Post_Claim_Route()
        {
            const string userId = "user-claim";
            var req = new ClaimProjectRequest();

            var svc = UserProjectServiceMock();
            svc.Setup(s => s.ClaimProjectAsync(userId, req))
               .Returns(Task.FromResult(SampleUserProject()));

            var cu = CurrentUserMock();
            cu.Setup(c => c.GetUserId()).Returns(userId);

            MyController<UserProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object, cu.Object))
                .Calling(c => c.ClaimProject(req))
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("POST"));

            // Route template via reflection
            var mi = typeof(UserProjectAssignmentsController)
                .GetMethod(nameof(UserProjectAssignmentsController.ClaimProject),
                           new[] { typeof(ClaimProjectRequest) });

            var httpPost = mi!.GetCustomAttribute<HttpPostAttribute>();
            Assert.NotNull(httpPost);
            Assert.Equal("claim", httpPost!.Template);
        }

        [Fact]
        public void ClaimProject_Should_Use_CurrentUser_And_Return_Ok()
        {
            const string userId = "user-claim";
            var req = new ClaimProjectRequest();
            var resultModel = SampleUserProject();

            var svc = UserProjectServiceMock();
            svc.Setup(s => s.ClaimProjectAsync(userId, req))
               .Returns(Task.FromResult(resultModel));

            var cu = CurrentUserMock();
            cu.Setup(c => c.GetUserId()).Returns(userId);

            MyController<UserProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object, cu.Object))
                .Calling(c => c.ClaimProject(req))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(resultModel));

            svc.VerifyAll();
            cu.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // GetUserProjects
        // ─────────────────────────────────────────────

        [Fact]
        public void GetUserProjects_Should_Have_Get_Mine_Route()
        {
            const string userId = "user-mine";

            var list = new List<UserProjectAssignmentResponse>
            {
                SampleUserProject(),
                SampleUserProject()
            };

            var svc = UserProjectServiceMock();
            svc.Setup(s => s.GetUserProjectsAsync(userId))
               .Returns(Task.FromResult<IEnumerable<UserProjectAssignmentResponse>>(list));

            var cu = CurrentUserMock();
            cu.Setup(c => c.GetUserId()).Returns(userId);

            MyController<UserProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object, cu.Object))
                .Calling(c => c.GetUserProjects())
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("GET"));

            var mi = typeof(UserProjectAssignmentsController)
                .GetMethod(nameof(UserProjectAssignmentsController.GetUserProjects),
                           Type.EmptyTypes);

            var httpGet = mi!.GetCustomAttribute<HttpGetAttribute>();
            Assert.NotNull(httpGet);
            Assert.Equal("mine", httpGet!.Template);
        }

        [Fact]
        public void GetUserProjects_Should_Use_CurrentUser_And_Return_Ok_With_List()
        {
            const string userId = "user-mine";

            var list = new List<UserProjectAssignmentResponse>
            {
                SampleUserProject(),
                SampleUserProject()
            };

            var svc = UserProjectServiceMock();
            svc.Setup(s => s.GetUserProjectsAsync(userId))
               .Returns(Task.FromResult<IEnumerable<UserProjectAssignmentResponse>>(list));

            var cu = CurrentUserMock();
            cu.Setup(c => c.GetUserId()).Returns(userId);

            MyController<UserProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object, cu.Object))
                .Calling(c => c.GetUserProjects())
                .ShouldReturn()
                .Ok(ok => ok.WithModel(list));

            svc.VerifyAll();
            cu.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // CompleteProject
        // ─────────────────────────────────────────────

        [Fact]
        public void CompleteProject_Should_Have_Post_Complete_Route()
        {
            const string userId = "user-complete";
            var projectId = Guid.NewGuid();
            var req = new CompleteUserProjectAssignmentRequest();

            var svc = UserProjectServiceMock();
            // We only care that the method is callable for attribute test
            svc.Setup(s => s.CompleteProjectAsync(userId, It.IsAny<CompleteUserProjectAssignmentRequest>()))
               .Returns(Task.FromResult(SampleUserProject(completed: true)));

            var cu = CurrentUserMock();
            cu.Setup(c => c.GetUserId()).Returns(userId);

            MyController<UserProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object, cu.Object))
                .Calling(c => c.CompleteProject(req, projectId))
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("POST"));

            var mi = typeof(UserProjectAssignmentsController)
                .GetMethod(nameof(UserProjectAssignmentsController.CompleteProject),
                           new[] { typeof(CompleteUserProjectAssignmentRequest), typeof(Guid) });

            var httpPost = mi!.GetCustomAttribute<HttpPostAttribute>();
            Assert.NotNull(httpPost);
            Assert.Equal("complete/{projectAssignmentId}", httpPost!.Template);
        }

        [Fact]
        public void CompleteProject_Should_Set_ProjectAssignmentId_From_Route_And_Return_Ok()
        {
            const string userId = "user-complete";
            var projectId = Guid.NewGuid();

            var req = new CompleteUserProjectAssignmentRequest(); // ProjectAssignmentId should be set in action
            var expected = SampleUserProject(completed: true);

            var svc = UserProjectServiceMock();
            svc.Setup(s => s.CompleteProjectAsync(
                    userId,
                    It.Is<CompleteUserProjectAssignmentRequest>(r => r.ProjectAssignmentId == projectId)))
               .Returns(Task.FromResult(expected));

            var cu = CurrentUserMock();
            cu.Setup(c => c.GetUserId()).Returns(userId);

            MyController<UserProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object, cu.Object))
                .Calling(c => c.CompleteProject(req, projectId))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(expected));

            svc.VerifyAll();
            cu.VerifyAll();
        }
    }
}
