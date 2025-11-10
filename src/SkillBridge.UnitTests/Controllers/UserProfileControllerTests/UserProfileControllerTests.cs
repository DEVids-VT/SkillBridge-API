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
using SkillBridge.Services.CurrentUser;
using SkillBridge.Services.UserProfile;
using Xunit;

namespace SkillBridge.UnitTests.Controllers.UserProfileControllerTests
{
    public class UserProfileControllerTests
    {
        private static Mock<IUserProfileService> UserProfileServiceMock(MockBehavior behavior = MockBehavior.Strict)
            => new(behavior);

        private static Mock<ICurrentUser> CurrentUserMock(MockBehavior behavior = MockBehavior.Loose)
            => new(behavior);

        private static UserProfileResponse SampleProfile(string? id = null) => new()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            // add more fields if your DTO requires them
        };

        // ─────────────────────────────────────────────
        // Controller-level attributes
        // ─────────────────────────────────────────────

        [Fact]
        public void Controller_Should_Have_ApiController_And_Correct_Route()
        {
            var svc = UserProfileServiceMock(MockBehavior.Loose).Object;
            var cu = CurrentUserMock().Object;

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc, cu))
                .ShouldHave()
                .Attributes(a => a
                    .ContainingAttributeOfType<ApiControllerAttribute>()
                    .SpecifyingRoute("api/u"));
        }

        // ─────────────────────────────────────────────
        // UPDATE (PUT {userId?}) – attributes
        // ─────────────────────────────────────────────

        [Fact]
        public void Update_Should_Have_Put_And_Candidate_Policy()
        {
            var userId = "user-1";
            var req = new UpdateUserProfileRequest();

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.UpdateAsync(req, userId))
               .Returns(Task.FromResult(SampleProfile(userId)));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.Update(userId, req))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("PUT")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(UserProfileController)
                .GetMethod(nameof(UserProfileController.Update),
                           new[] { typeof(string), typeof(UpdateUserProfileRequest) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("Candidate", au!.Policy);
        }

        [Fact]
        public void Update_Should_Return_Ok_With_Model()
        {
            var userId = "user-1";
            var req = new UpdateUserProfileRequest();
            var profile = SampleProfile(userId);

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.UpdateAsync(req, userId))
               .Returns(Task.FromResult(profile));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.Update(userId, req))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(profile));

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // CVUploadAsync (PATCH {userId?}/cv) – attributes + behavior
        // ─────────────────────────────────────────────

        [Fact]
        public void CVUploadAsync_Should_Have_Patch_And_Candidate_Policy()
        {
            var userId = "user-2";
            var req = new CVUploadRequest();

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.UpdateCVUpload(req, userId))
               .Returns(Task.FromResult(SampleProfile(userId)));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.CVUploadAsync(userId, req))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("PATCH")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(UserProfileController)
                .GetMethod(nameof(UserProfileController.CVUploadAsync),
                           new[] { typeof(string), typeof(CVUploadRequest) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("Candidate", au!.Policy);
        }

        [Fact]
        public void CVUploadAsync_Should_Return_Ok_With_Model()
        {
            var userId = "user-2";
            var req = new CVUploadRequest();
            var profile = SampleProfile(userId);

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.UpdateCVUpload(req, userId))
               .Returns(Task.FromResult(profile));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.CVUploadAsync(userId, req))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(profile));

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // ProfilePictureAsync (PATCH {userId?}/profile-picture)
        // ─────────────────────────────────────────────

        [Fact]
        public void ProfilePictureAsync_Should_Have_Patch_And_Candidate_Policy()
        {
            var userId = "user-3";
            var req = new ProfilePictureRequest();

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.UpdateProfilePicture(req, userId))
               .Returns(Task.FromResult(SampleProfile(userId)));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.ProfilePictureAsync(userId, req))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("PATCH")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(UserProfileController)
                .GetMethod(nameof(UserProfileController.ProfilePictureAsync),
                           new[] { typeof(string), typeof(ProfilePictureRequest) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("Candidate", au!.Policy);
        }

        [Fact]
        public void ProfilePictureAsync_Should_Return_Ok_With_Model()
        {
            var userId = "user-3";
            var req = new ProfilePictureRequest();
            var profile = SampleProfile(userId);

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.UpdateProfilePicture(req, userId))
               .Returns(Task.FromResult(profile));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.ProfilePictureAsync(userId, req))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(profile));

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // Create (POST {userId?}) – attributes + behavior
        // ─────────────────────────────────────────────

        [Fact]
        public void Create_Should_Have_Post_And_Candidate_Policy()
        {
            var userId = "user-4";
            var req = new CreateUserProfileRequest();
            var created = SampleProfile(userId);

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.CreateAsync(req, userId))
               .Returns(Task.FromResult(created));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.Create(userId, req))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("POST")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(UserProfileController)
                .GetMethod(nameof(UserProfileController.Create),
                           new[] { typeof(string), typeof(CreateUserProfileRequest) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("Candidate", au!.Policy);
        }

        [Fact]
        public void Create_Should_Return_CreatedAtAction_To_GetMyProfile()
        {
            var userId = "user-4";
            var req = new CreateUserProfileRequest();
            var created = SampleProfile(userId);

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.CreateAsync(req, userId))
               .Returns(Task.FromResult(created));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.Create(userId, req))
                .ShouldReturn()
                .Created(cr => cr
                    .AtAction(nameof(UserProfileController.GetMyProfile))
                    .Passing(result =>
                    {
                        var r = Assert.IsType<CreatedAtActionResult>(result);
                        Assert.Equal(created.Id, r.RouteValues["userId"]);
                        Assert.Same(created, r.Value);
                    }));

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // GetMyProfile (GET {userId?}) – attributes + behavior
        // ─────────────────────────────────────────────

        [Fact]
        public void GetMyProfile_Should_Have_Get_And_Authorize()
        {
            var userId = "user-5";

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.GetMyProfileAsync(userId))
               .Returns(Task.FromResult(SampleProfile(userId)));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.GetMyProfile(userId))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("GET")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(UserProfileController)
                .GetMethod(nameof(UserProfileController.GetMyProfile),
                           new[] { typeof(string) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            // plain [Authorize] with no Policy expected
            Assert.Null(au!.Policy);
        }

        [Fact]
        public void GetMyProfile_Should_Return_Ok_With_Model()
        {
            var userId = "user-5";
            var profile = SampleProfile(userId);

            var svc = UserProfileServiceMock();
            svc.Setup(s => s.GetMyProfileAsync(userId))
               .Returns(Task.FromResult(profile));

            MyController<UserProfileController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.GetMyProfile(userId))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(profile));

            svc.VerifyAll();
        }
    }
}
