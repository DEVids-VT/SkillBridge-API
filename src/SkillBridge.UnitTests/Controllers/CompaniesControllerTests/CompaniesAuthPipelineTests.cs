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
using SkillBridge.Services.Company;
using SkillBridge.Services.CurrentUser;
using Xunit;

namespace SkillBridge.UnitTests.Controllers.CompaniesControllerTests
{
    // NOTE: Despite the file/class name, these are *controller-style* auth tests
    // (no pipeline). We assert verb + [Authorize] presence via MyTested and
    // the Policy value via reflection. Minimal service setups prevent throws.

    public class CompaniesAuthPipelineTests
    {
        private static Mock<ICompanyService> CompanyMock() => new(MockBehavior.Strict);
        private static Mock<ICurrentUser> CurrentUserMock() => new(MockBehavior.Loose);

        private static CompanyResponse FakeCompany(Guid? id = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            Name = "ACME"
        };

        // ─────────────────────────────────────────────────────────────
        // CREATE  [HttpPost] + [Authorize(Policy = "CompanyScope")]
        // ─────────────────────────────────────────────────────────────
        [Fact]
        public void Create_Should_Have_Post_And_CompanyScope_Policy()
        {
            var svc = CompanyMock();
            svc.Setup(s => s.CreateAsync(It.IsAny<CreateCompanyRequest>()))
               .ReturnsAsync(FakeCompany());

            MyController<CompaniesController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.Create(new CreateCompanyRequest { Name = "X" }))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("POST")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(CompaniesController)
                .GetMethod(nameof(CompaniesController.Create),
                           new[] { typeof(CreateCompanyRequest) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("CompanyScope", au!.Policy);
        }

        // ─────────────────────────────────────────────────────────────
        // GET /my  [Authorize] (no specific policy)
        // ─────────────────────────────────────────────────────────────
        [Fact]
        public void GetMyCompany_Should_Have_Authorize_Attribute()
        {
            var svc = CompanyMock();
            svc.Setup(s => s.GetMyCompanyAsync(It.IsAny<string?>()))
               .ReturnsAsync(FakeCompany());

            MyController<CompaniesController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.GetMyCompany(null))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("GET")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(CompaniesController)
                .GetMethod(nameof(CompaniesController.GetMyCompany),
                           new[] { typeof(string) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Null(au!.Policy); // plain [Authorize] without Policy is expected
        }

        // ─────────────────────────────────────────────────────────────
        // UPDATE  [HttpPut("{id}")] + [Authorize(Policy = "Company")]
        // ─────────────────────────────────────────────────────────────
        [Fact]
        public void Update_Should_Have_Put_And_Company_Policy()
        {
            var id = Guid.NewGuid();

            var svc = CompanyMock();
            svc.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateCompanyRequest>()))
               .ReturnsAsync(FakeCompany(id));

            MyController<CompaniesController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.Update(id, new UpdateCompanyRequest { Name = "Y" }))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("PUT")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(CompaniesController)
                .GetMethod(nameof(CompaniesController.Update),
                           new[] { typeof(Guid), typeof(UpdateCompanyRequest) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("Company", au!.Policy);
        }

        // ─────────────────────────────────────────────────────────────
        // DELETE  [HttpDelete("{id}")] + [Authorize(Policy = "CompaniesScope")]
        // ─────────────────────────────────────────────────────────────
        [Fact]
        public void Delete_Should_Have_Delete_And_CompaniesScope_Policy()
        {
            var id = Guid.NewGuid();

            var svc = CompanyMock();
            svc.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            MyController<CompaniesController>
                .Instance(i => i.WithDependencies(svc.Object, CurrentUserMock().Object))
                .Calling(c => c.Delete(id))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("DELETE")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(CompaniesController)
                .GetMethod(nameof(CompaniesController.Delete),
                           new[] { typeof(Guid) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("CompaniesScope", au!.Policy);
        }

        // ─────────────────────────────────────────────────────────────
        // PATCH /{id?}/companyLogo  [Authorize(Policy = "Company")]
        // ─────────────────────────────────────────────────────────────
        [Fact]
        public void UpdateCompanyLogo_Should_Have_Patch_And_Company_Policy()
        {
            var id = Guid.NewGuid();

            var svc = new Mock<ICompanyService>(MockBehavior.Strict);
            svc.Setup(s => s.UpdateCompanyLogoAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<UpdateCompanyLogoRequest>()))
               .Returns(Task.FromResult(new CompanyResponse
               {
                   Id = Guid.NewGuid(),
                   Name = "ACME"
                   // fill any other required properties here if your DTO enforces them
               }));

            MyController<CompaniesController>
                .Instance(i => i.WithDependencies(svc.Object, new Mock<ICurrentUser>(MockBehavior.Loose).Object))
                .Calling(c => c.UpdateCompanyLogo(id, new UpdateCompanyLogoRequest()))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("PATCH")
                    .ContainingAttributeOfType<AuthorizeAttribute>());

            var mi = typeof(CompaniesController)
                .GetMethod(nameof(CompaniesController.UpdateCompanyLogo),
                           new[] { typeof(Guid), typeof(UpdateCompanyLogoRequest) });

            var au = mi!.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(au);
            Assert.Equal("Company", au!.Policy);
        }
    }
}
