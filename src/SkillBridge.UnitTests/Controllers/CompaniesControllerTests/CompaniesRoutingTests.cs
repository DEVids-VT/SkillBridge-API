using SkillBridge.Controllers;
using System;
using System.Net.Http;
using MyTested.AspNetCore.Mvc;
using Xunit;
using HttpMethod = System.Net.Http.HttpMethod;
using SkillBridge.Models.Request;

namespace SkillBridge.UnitTests.Controllers.CompaniesControllerTests
{
    public class CompaniesRoutingTests
    {
        #region POST /api/c
        [Fact]
        public void Create_Route_Should_Map()
            => MyRouting.Configuration()
                .ShouldMap(request => request
                    .WithPath("/api/c")
                    .WithMethod("POST"))
                .To<CompaniesController>(c => c.Create(With.Any<CreateCompanyRequest>()));
        #endregion

        #region GET /api/c/id/{id:guid}
        [Fact]
        public void GetById_Route_Should_Map()
        {
            var id = Guid.NewGuid();

            MyRouting.Configuration()
                .ShouldMap($"/api/c/id/{id}")
                .To<CompaniesController>(c => c.GetById(id));
        }
        #endregion

        #region GET /api/c/my?userId=...
        [Fact]
        public void GetMyCompany_Route_Should_Map_With_Query()
     => MyRouting.Configuration()
         .ShouldMap("/api/c/my?userId=auth0%7C123")       // ← encode '|'
         .To<CompaniesController>(c => c.GetMyCompany("auth0|123"));
        #endregion

        #region GET /api/c
        [Fact]
        public void GetAll_Route_Should_Map()
            => MyRouting.Configuration()
                .ShouldMap("/api/c")
                .To<CompaniesController>(c => c.GetAll());
        #endregion

        #region PUT /api/c/{id}
        [Fact]
        public void Update_Route_Should_Map()
        {
            var id = Guid.NewGuid();

            MyRouting.Configuration()
                .ShouldMap(req => req
                    .WithPath($"/api/c/{id}")
                    .WithMethod("PUT"))
                .To<CompaniesController>(c => c.Update(id, With.Any<UpdateCompanyRequest>()));
        }
        #endregion

        #region DELETE /api/c/{id}
        [Fact]
        public void Delete_Route_Should_Map()
        {
            var id = Guid.NewGuid();

            MyRouting.Configuration()
                .ShouldMap(req => req
                    .WithPath($"/api/c/{id}")
                    .WithMethod("DELETE"))
                .To<CompaniesController>(c => c.Delete(id));
        }
        #endregion

        #region PATCH /api/c/{id}/companyLogo
        [Fact]
        public void UpdateCompanyLogo_Route_Should_Map()
        {
            var id = Guid.NewGuid();

            MyRouting.Configuration()
                .ShouldMap(req => req
                    .WithPath($"/api/c/{id}/companyLogo")
                    .WithMethod("PATCH"))
                .To<CompaniesController>(c => c.UpdateCompanyLogo(id, With.Any<UpdateCompanyLogoRequest>()));
        }
        #endregion
    }
}
