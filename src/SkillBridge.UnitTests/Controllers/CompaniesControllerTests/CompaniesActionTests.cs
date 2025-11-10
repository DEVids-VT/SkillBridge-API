using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class CompaniesActionTests
    {
        private readonly Mock<ICompanyService> _svc = new();
        private readonly Mock<ICurrentUser> _currentUser = new();

        [Fact]
        public void Create_Should_Return_201_Created_With_Model()
        {
            var created = new CompanyResponse { Id = Guid.NewGuid(), Name = "NewCo" };

            _svc.Setup(s => s.CreateAsync(It.IsAny<CreateCompanyRequest>()))
                .ReturnsAsync(created);

            MyMvc.Controller<CompaniesController>(c => c
                    .WithDependencies(_svc.Object, _currentUser.Object)
                    .WithUser(u => u.WithClaim("scope", "CompanyScope")))
                .Calling(c => c.Create(new CreateCompanyRequest { Name = "NewCo" }))
                .ShouldReturn()
                .Created(cr => cr
                    .AtAction(nameof(CompaniesController.GetById))
                    .WithModelOfType<CompanyResponse>()
                    .Passing(m => m.Id == created.Id && m.Name == "NewCo"));
        }

        [Fact]
        public void GetById_Should_Return_200_Ok_With_Model()
        {
            var id = Guid.NewGuid();

            _svc.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new CompanyResponse { Id = id, Name = "Contoso" });

            MyMvc.Controller<CompaniesController>(c => c
                    .WithDependencies(_svc.Object, _currentUser.Object))
                .Calling(c => c.GetById(id))
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<CompanyResponse>()
                    .Passing(m => m.Id == id && m.Name == "Contoso"));
        }

        [Fact]
        public void GetMyCompany_Should_Return_200_Ok_When_Authenticated()
        {
            _svc.Setup(s => s.GetMyCompanyAsync(null))
                .ReturnsAsync(new CompanyResponse { Id = Guid.NewGuid(), Name = "Mine" });

            MyMvc.Controller<CompaniesController>(c => c
                    .WithDependencies(_svc.Object, _currentUser.Object)
                    .WithUser()) // any authenticated user is fine
                .Calling(c => c.GetMyCompany(null))
                .ShouldReturn()
                .Ok(ok => ok.WithModelOfType<CompanyResponse>());
        }

        [Fact]
        public void GetAll_Should_Return_200_Ok_With_List()
        {
            _svc.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new[]
                {
                new CompanyResponse { Id = Guid.NewGuid(), Name = "A" },
                new CompanyResponse { Id = Guid.NewGuid(), Name = "B" }
                }.AsEnumerable());

            MyMvc.Controller<CompaniesController>(c => c
                    .WithDependencies(_svc.Object, _currentUser.Object))
                .Calling(c => c.GetAll())
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<IEnumerable<CompanyResponse>>()
                    .Passing(list => list.Count() == 2));
        }

        [Fact]
        public void Update_Should_Return_200_Ok_When_Authorized()
        {
            var id = Guid.NewGuid();

            _svc.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateCompanyRequest>()))
                .ReturnsAsync(new CompanyResponse { Id = id, Name = "Updated" });

            MyMvc.Controller<CompaniesController>(c => c
                    .WithDependencies(_svc.Object, _currentUser.Object)
                    .WithUser(u => u.WithClaim("scope", "Company"))) // adjust to your actual policy requirement
                .Calling(c => c.Update(id, new UpdateCompanyRequest { Name = "Updated" }))
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<CompanyResponse>()
                    .Passing(m => m.Id == id && m.Name == "Updated"));
        }

        [Fact]
        public void Delete_Should_Return_204_NoContent_When_Authorized()
        {
            var id = Guid.NewGuid();

            _svc.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            MyMvc.Controller<CompaniesController>(c => c
                    .WithDependencies(_svc.Object, _currentUser.Object)
                    .WithUser(u => u.WithClaim("scope", "CompaniesScope")))
                .Calling(c => c.Delete(id))
                .ShouldReturn()
                .NoContent();
        }

        [Fact]
        public void UpdateCompanyLogo_Should_Return_200_Ok_When_Authorized()
        {
            var id = Guid.NewGuid();

            _svc.Setup(s => s.UpdateCompanyLogoAsync(id, It.IsAny<UpdateCompanyLogoRequest>()))
                .ReturnsAsync(new CompanyResponse { Id = id, Name = "LogoUpdated" });

            MyMvc.Controller<CompaniesController>(c => c
                    .WithDependencies(_svc.Object, _currentUser.Object)
                    .WithUser(u => u.WithClaim("scope", "Company")))
                .Calling(c => c.UpdateCompanyLogo(id, new UpdateCompanyLogoRequest()))
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<CompanyResponse>()
                    .Passing(m => m.Id == id));
        }
    }
}
