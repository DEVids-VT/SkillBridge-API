using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTested.AspNetCore.Mvc;
using SkillBridge.Controllers;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.Skill;
using Xunit;

namespace SkillBridge.UnitTests.Controllers.SkillsControllerTests
{
    public class SkillsControllerTests
    {
        private static Mock<ISkillService> SkillServiceMock(MockBehavior behavior = MockBehavior.Strict)
            => new(behavior);

        private static SkillResponse SampleSkill(Guid? id = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            Name = "C#"
            // add other properties if your DTO requires them
        };

        // ─────────────────────────────────────────────
        // Controller-level attributes
        // ─────────────────────────────────────────────

        [Fact]
        public void Controller_Should_Have_ApiController_And_Route()
        {
            var svc = SkillServiceMock(MockBehavior.Loose).Object;

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc))
                .ShouldHave()
                .Attributes(a => a
                    .ContainingAttributeOfType<ApiControllerAttribute>()
                    .SpecifyingRoute("api/[controller]"));
        }

        // ─────────────────────────────────────────────
        // Create
        // ─────────────────────────────────────────────

        [Fact]
        public void Create_Should_Have_Post_Attribute()
        {
            var svc = SkillServiceMock();
            svc.Setup(s => s.CreateAsync(It.IsAny<CreateSkillRequest>()))
               .Returns(Task.FromResult(SampleSkill()));

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Create(new CreateSkillRequest()))
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("POST"));
        }

        [Fact]
        public void Create_Should_Return_CreatedAtAction()
        {
            var request = new CreateSkillRequest { Name = "C#" };
            var created = SampleSkill();

            var svc = SkillServiceMock();
            svc.Setup(s => s.CreateAsync(request)).Returns(Task.FromResult(created));

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Create(request))
                .ShouldReturn()
                .Created(cr => cr
                    .AtAction(nameof(SkillsController.GetById))
                    .Passing(result =>
                    {
                        var r = Assert.IsType<CreatedAtActionResult>(result);
                        Assert.Equal(created.Id, r.RouteValues["id"]);
                        Assert.Same(created, r.Value);
                    }));

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // GetById
        // ─────────────────────────────────────────────

        [Fact]
        public void GetById_Should_Have_Get_Attribute()
        {
            var id = Guid.NewGuid();

            var svc = SkillServiceMock();
            svc.Setup(s => s.GetByIdAsync(id)).Returns(Task.FromResult(SampleSkill(id)));

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.GetById(id))
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("GET"));
        }

        [Fact]
        public void GetById_Should_Return_Ok_With_Model()
        {
            var id = Guid.NewGuid();
            var expected = SampleSkill(id);

            var svc = SkillServiceMock();
            svc.Setup(s => s.GetByIdAsync(id)).Returns(Task.FromResult(expected));

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.GetById(id))
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<SkillResponse>()
                    .Passing(m => m.Id == expected.Id));

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // GetAll
        // ─────────────────────────────────────────────

        [Fact]
        public void GetAll_Should_Have_Get_Attribute()
        {
            var svc = SkillServiceMock();
            svc.Setup(s => s.GetAllAsync())
               .Returns(Task.FromResult<IEnumerable<SkillResponse>>(new List<SkillResponse>()));

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.GetAll())
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("GET"));
        }

        [Fact]
        public void GetAll_Should_Return_Ok_With_List()
        {
            var data = new List<SkillResponse> { SampleSkill(), SampleSkill() };

            var svc = SkillServiceMock();
            svc.Setup(s => s.GetAllAsync())
               .Returns(Task.FromResult<IEnumerable<SkillResponse>>(data));

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.GetAll())
                .ShouldReturn()
                .Ok(ok => ok.WithModel(data));

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // Update
        // ─────────────────────────────────────────────

        [Fact]
        public void Update_Should_Have_Put_Attribute()
        {
            var id = Guid.NewGuid();
            var svc = SkillServiceMock();
            svc.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateSkillRequest>()))
               .Returns(Task.FromResult(SampleSkill(id)));

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Update(id, new UpdateSkillRequest()))
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("PUT"));
        }

        [Fact]
        public void Update_Should_Return_Ok_With_Model()
        {
            var id = Guid.NewGuid();
            var req = new UpdateSkillRequest { Name = "Updated" };
            var updated = SampleSkill(id);

            var svc = SkillServiceMock();
            svc.Setup(s => s.UpdateAsync(id, req)).Returns(Task.FromResult(updated));

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Update(id, req))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(updated));

            svc.VerifyAll();
        }

        // ─────────────────────────────────────────────
        // Delete
        // ─────────────────────────────────────────────

        [Fact]
        public void Delete_Should_Have_Delete_Attribute()
        {
            var id = Guid.NewGuid();
            var svc = SkillServiceMock();
            svc.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Delete(id))
                .ShouldHave()
                .ActionAttributes(a => a.RestrictingForHttpMethod("DELETE"));
        }

        [Fact]
        public void Delete_Should_Return_NoContent()
        {
            var id = Guid.NewGuid();
            var svc = SkillServiceMock();
            svc.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            MyController<SkillsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Delete(id))
                .ShouldReturn()
                .NoContent();

            svc.VerifyAll();
        }
    }
}
