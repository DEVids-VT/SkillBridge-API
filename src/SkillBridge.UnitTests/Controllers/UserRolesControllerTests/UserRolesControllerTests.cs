using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTested.AspNetCore.Mvc;
using SkillBridge.Controllers;
using SkillBridge.Services.UserRole;
using Xunit;

namespace SkillBridge.UnitTests.Controllers.UserRolesControllerTests
{
    public class UserRolesControllerTests
    {
        private static Mock<IUserRoleService> RoleServiceMock(MockBehavior behavior = MockBehavior.Strict)
            => new(behavior);

        // ─────────────────────────────────────────────
        // Controller-level attributes
        // ─────────────────────────────────────────────

        [Fact]
        public void Controller_Should_Have_ApiController_Authorize_And_Route()
        {
            var svc = RoleServiceMock(MockBehavior.Loose).Object;

            MyController<UserRolesController>
                .Instance(i => i.WithDependencies(svc))
                .ShouldHave()
                .Attributes(a => a
                    .ContainingAttributeOfType<ApiControllerAttribute>()
                    .ContainingAttributeOfType<AuthorizeAttribute>()
                    .SpecifyingRoute("api/r"));
        }

        // ─────────────────────────────────────────────
        // BecomeCompany
        // ─────────────────────────────────────────────

        [Fact]
        public void BecomeCompany_Should_Have_Post_With_Correct_Route()
        {
            var svc = RoleServiceMock();

            // Explicit argument instead of relying on optional param
            svc.Setup(s => s.BecomeCompanyAsync(It.IsAny<string>()))
               .Returns(Task.FromResult(true));

            MyController<UserRolesController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.BecomeCompany())
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("POST"));

            // Route template via reflection
            var mi = typeof(UserRolesController)
                .GetMethod(nameof(UserRolesController.BecomeCompany));

            var httpPost = mi!.GetCustomAttribute<HttpPostAttribute>();
            Assert.NotNull(httpPost);
            Assert.Equal("become-company", httpPost!.Template);
        }

        [Fact]
        public void BecomeCompany_Should_Return_Ok_When_Service_Returns_True()
        {
            var svc = RoleServiceMock();

            // Controller calls BecomeCompanyAsync() → default userId = null
            // We can be precise and match null:
            svc.Setup(s => s.BecomeCompanyAsync(null))
               .Returns(Task.FromResult(true));

            MyController<UserRolesController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.BecomeCompany())
                .ShouldReturn()
                .Ok();

            svc.VerifyAll();
        }

        [Fact]
        public void BecomeCompany_Should_Return_BadRequest_When_Service_Returns_False()
        {
            var svc = RoleServiceMock();

            svc.Setup(s => s.BecomeCompanyAsync(null))
               .Returns(Task.FromResult(false));

            MyController<UserRolesController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.BecomeCompany())
                .ShouldReturn()
                .BadRequest();

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // BecomeCandidate
        // ─────────────────────────────────────────────

        [Fact]
        public void BecomeCandidate_Should_Have_Post_With_Correct_Route()
        {
            var svc = RoleServiceMock();
            svc.Setup(s => s.BecomeCandidateAsync(It.IsAny<string>()))
               .Returns(Task.FromResult(true));

            MyController<UserRolesController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.BecomeCandidate())
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("POST"));

            var mi = typeof(UserRolesController)
                .GetMethod(nameof(UserRolesController.BecomeCandidate));

            var httpPost = mi!.GetCustomAttribute<HttpPostAttribute>();
            Assert.NotNull(httpPost);
            Assert.Equal("become-candidate", httpPost!.Template);
        }

        [Fact]
        public void BecomeCandidate_Should_Return_Ok_When_Service_Returns_True()
        {
            var svc = RoleServiceMock();
            svc.Setup(s => s.BecomeCandidateAsync(null))
               .Returns(Task.FromResult(true));

            MyController<UserRolesController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.BecomeCandidate())
                .ShouldReturn()
                .Ok();

            svc.VerifyAll();
        }

        [Fact]
        public void BecomeCandidate_Should_Return_BadRequest_When_Service_Returns_False()
        {
            var svc = RoleServiceMock();
            svc.Setup(s => s.BecomeCandidateAsync(null))
               .Returns(Task.FromResult(false));

            MyController<UserRolesController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.BecomeCandidate())
                .ShouldReturn()
                .BadRequest();

            svc.VerifyAll();
        }
    }
}
