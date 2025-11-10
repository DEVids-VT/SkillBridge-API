using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.Controllers;
using SkillBridge.Models.Request;
using Xunit;

namespace SkillBridge.UnitTests.Controllers.CompaniesControllerTests
{
    public class CompaniesAttributesTests
    {
        #region Controller-level attributes

        [Fact]
        public void Controller_Should_Have_ApiController_And_Route()
        {
            var type = typeof(CompaniesController);

            Assert.True(type.GetCustomAttributes(typeof(ApiControllerAttribute), inherit: true).Any());

            var route = type.GetCustomAttributes(typeof(RouteAttribute), inherit: true)
                            .Cast<RouteAttribute>()
                            .FirstOrDefault();

            Assert.NotNull(route);
            Assert.Equal("api/c", route!.Template);
        }

        #endregion

        #region Action-level attributes

        [Fact]
        public void Create_Should_Have_HttpPost_And_Authorize_CompanyScope()
        {
            var mi = typeof(CompaniesController).GetMethod(nameof(CompaniesController.Create))!;
            Assert.True(mi.GetCustomAttributes(typeof(HttpPostAttribute), true).Any());

            var authorize = mi.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                              .Cast<AuthorizeAttribute>()
                              .FirstOrDefault();

            Assert.NotNull(authorize);
            Assert.Equal("CompanyScope", authorize!.Policy);
        }

        [Fact]
        public void GetById_Should_Have_HttpGet_With_Guid_Template()
        {
            var mi = typeof(CompaniesController).GetMethod(nameof(CompaniesController.GetById))!;
            var httpGet = mi.GetCustomAttributes(typeof(HttpGetAttribute), true)
                            .Cast<HttpGetAttribute>()
                            .FirstOrDefault();

            Assert.NotNull(httpGet);
            Assert.Equal("id/{id:guid}", httpGet!.Template);
        }

        [Fact]
        public void GetMyCompany_Should_Have_HttpGet_And_Authorize()
        {
            var mi = typeof(CompaniesController).GetMethod(nameof(CompaniesController.GetMyCompany))!;
            Assert.True(mi.GetCustomAttributes(typeof(HttpGetAttribute), true).Any());
            Assert.True(mi.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any());
        }

        [Fact]
        public void GetAll_Should_Have_HttpGet()
        {
            var mi = typeof(CompaniesController).GetMethod(nameof(CompaniesController.GetAll))!;
            Assert.True(mi.GetCustomAttributes(typeof(HttpGetAttribute), true).Any());
        }

        [Fact]
        public void Update_Should_Have_HttpPut_And_Authorize_Company()
        {
            var mi = typeof(CompaniesController).GetMethod(nameof(CompaniesController.Update))!;
            Assert.True(mi.GetCustomAttributes(typeof(HttpPutAttribute), true).Any());

            var authorize = mi.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                              .Cast<AuthorizeAttribute>()
                              .FirstOrDefault();

            Assert.NotNull(authorize);
            Assert.Equal("Company", authorize!.Policy);
        }

        [Fact]
        public void Delete_Should_Have_HttpDelete_And_Authorize_CompaniesScope()
        {
            var mi = typeof(CompaniesController).GetMethod(nameof(CompaniesController.Delete))!;
            Assert.True(mi.GetCustomAttributes(typeof(HttpDeleteAttribute), true).Any());

            var authorize = mi.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                              .Cast<AuthorizeAttribute>()
                              .FirstOrDefault();

            Assert.NotNull(authorize);
            Assert.Equal("CompaniesScope", authorize!.Policy);
        }

        [Fact]
        public void UpdateCompanyLogo_Should_Have_HttpPatch_And_Authorize_Company()
        {
            var mi = typeof(CompaniesController).GetMethod(nameof(CompaniesController.UpdateCompanyLogo))!;
            Assert.True(mi.GetCustomAttributes(typeof(HttpPatchAttribute), true).Any());

            var authorize = mi.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                              .Cast<AuthorizeAttribute>()
                              .FirstOrDefault();

            Assert.NotNull(authorize);
            Assert.Equal("Company", authorize!.Policy);
        }

        #endregion
    }
}
