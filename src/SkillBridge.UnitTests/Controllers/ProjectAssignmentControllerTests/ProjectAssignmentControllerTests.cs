using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyTested.AspNetCore.Mvc;
using SkillBridge.Controllers;
using SkillBridge.Models.Enums;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.ProjectAssignment;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EntitiesProjectAssignment = SkillBridge.Models.Entities.ProjectAssignment;
using SpecOfProject = SkillBridge.Models.Specifications.Specification<SkillBridge.Models.Entities.ProjectAssignment>;
// Aliases to avoid type/name conflicts and to be explicit in mocks
using TestHttpMethod = MyTested.AspNetCore.Mvc.HttpMethod;

namespace SkillBridge.Tests.Controllers
{
    [Collection("App")]
    public class ProjectAssignmentsControllerTests
    {
        private static Mock<SkillBridge.Services.ProjectAssignment.IProjectAssignmentService> ServiceMock()
            => new(MockBehavior.Strict);

        private static ProjectAssignmentResponse SampleProject(Guid? id = null)
    => new()
    {
        Id = id ?? Guid.NewGuid(),
        Title = "Sample",
        CompanyName = "ACME",
        Level = ProjectAssignmentLevel.Intermediate // <-- enum, not string
        // NOTE: ProjectAssignmentResponse has no CompanySector/ProjectSkills
    };
        private static AssignmentTaskResponse SampleTask(Guid? id = null)
            => new()
            {
                Id = id ?? Guid.NewGuid(),
                Title = "Task 1",
                Description = "Do thing",
                IsCompleted = false
            };

        // -----------------------------
        // Routing + attributes
        // -----------------------------

        private static IProjectAssignmentService Svc()
        => new Mock<IProjectAssignmentService>(MockBehavior.Loose).Object;

        [Fact]
        public void Controller_Should_Have_Correct_Route_Prefix()
       => MyController<ProjectAssignmentsController>
           .Instance(i => i.WithDependencies(Svc()))           // <<< supply dependency
           .ShouldHave()
           .Attributes(a => a.SpecifyingRoute("api/p"));

        [Fact]
        public void Create_ControllerStyle_Should_Execute_And_Return_Created()
        {
            var companyId = Guid.NewGuid();
            var req = new CreateProjectAssignmentRequest { Title = "X" };
            var created = new ProjectAssignmentResponse { Id = Guid.NewGuid(), Title = "X", CompanyName = "ACME" };

            var svc = new Mock<IProjectAssignmentService>(MockBehavior.Loose);
            svc.Setup(s => s.CreateAsync(companyId, req)).Returns(Task.FromResult(created));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Create(companyId, req))
                .ShouldReturn()
                .Created(cr => cr.AtAction(nameof(ProjectAssignmentsController.GetById)));
        }

        [Fact]
        public void UpdateTask_Route_Template_Should_Be_Reachable()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            MyRouting
                .Configuration()
                .ShouldMap(req => req
                    .WithLocation($"/api/p/{projectId}/tasks/{taskId}")
                    .WithMethod("PUT"));   // must match [HttpPut]
        }

        [Fact]
        public void CompleteTask_Should_Have_Patch_And_Candidate_Policy()
        {
            var svc = new Mock<IProjectAssignmentService>(MockBehavior.Loose);
            svc.Setup(s => s.CompleteTaskAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
               .Returns(Task.FromResult(new AssignmentTaskResponse { Id = Guid.NewGuid(), IsCompleted = true }));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.CompleteTask(Guid.NewGuid(), Guid.NewGuid()))
                .ShouldHave()
                .ActionAttributes(a => a
                    .RestrictingForHttpMethod("PATCH")
                    .ContainingAttributeOfType<AuthorizeAttribute>());
        }

        [Fact]
        public void CompleteTask_ControllerStyle_Should_Execute()
        {
            var svc = new Mock<IProjectAssignmentService>(MockBehavior.Loose);
            svc.Setup(s => s.CompleteTaskAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
               .Returns(Task.FromResult(new AssignmentTaskResponse { Id = Guid.NewGuid(), IsCompleted = true }));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.CompleteTask(Guid.NewGuid(), Guid.NewGuid()))
                .ShouldReturn()
                .Ok();
        }

        // -----------------------------
        // Behavior tests
        // -----------------------------

        [Fact]
        public void GetById_Should_Return_Ok_With_Model()
        {
            var id = Guid.NewGuid();
            var expected = SampleProject(id);

            var svc = ServiceMock();
            svc.Setup(s => s.GetByIdAsync(id))
               .Returns(Task.FromResult(expected));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.GetById(id))
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<ProjectAssignmentResponse>()
                    .Passing(m => m.Id == expected.Id));

            svc.VerifyAll();
        }

        [Fact]
        public void Create_Should_Return_CreatedAtAction()
        {
            var companyId = Guid.NewGuid();
            var created = SampleProject();
            var request = new CreateProjectAssignmentRequest { Title = "New" };

            var svc = ServiceMock();
            svc.Setup(s => s.CreateAsync(companyId, request))
               .Returns(Task.FromResult(created));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Create(companyId, request))
                .ShouldReturn()
                .Created(createdAt => createdAt
                    .AtAction(nameof(ProjectAssignmentsController.GetById))
                    .Passing(result =>
                    {
                        var r = Assert.IsType<CreatedAtActionResult>(result);
                        Assert.Equal(created.Id, r.RouteValues["id"]);
                        Assert.Same(created, r.Value);
                    }));

            svc.VerifyAll();
        }

        [Fact]
        public void Controller_Should_Have_Route_Prefix()
        {
            var svc = new Mock<IProjectAssignmentService>(MockBehavior.Loose).Object;

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc))
                .ShouldHave()
                .Attributes(a => a.SpecifyingRoute("api/p"));
        }


        [Fact]
        public void GetByCompanyId_Should_Return_Ok_With_List()
        {
            var companyId = Guid.NewGuid();
            var data = new List<ProjectAssignmentResponse> { SampleProject(), SampleProject() };

            var svc = ServiceMock();
            svc.Setup(s => s.GetByCompanyIdAsync(companyId))
               .Returns(Task.FromResult<IEnumerable<ProjectAssignmentResponse>>(data));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.GetByCompanyId(companyId))
                .ShouldReturn()
                .Ok(ok => ok.WithModelOfType<IEnumerable<ProjectAssignmentResponse>>());

            svc.VerifyAll();
        }

        [Fact]
        public void Update_Should_Return_Ok_With_Model()
        {
            var id = Guid.NewGuid();
            var req = new UpdateProjectAssignmentRequest { Title = "Updated" };
            var updated = SampleProject(id);

            var svc = ServiceMock();
            svc.Setup(s => s.UpdateAsync(id, req))
               .Returns(Task.FromResult(updated));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Update(id, req))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(updated));

            svc.VerifyAll();
        }

        [Fact]
        public void Delete_Should_Return_NoContent()
        {
            var id = Guid.NewGuid();

            var svc = ServiceMock();
            svc.Setup(s => s.DeleteAsync(id))
               .Returns(Task.CompletedTask);

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Delete(id))
                .ShouldReturn()
                .NoContent();

            svc.VerifyAll();
        }

        [Fact]
        public void CreateTask_Should_Return_CreatedAtAction()
        {
            var projectId = Guid.NewGuid();
            var req = new CreateAssignmentTaskRequest { Title = "Task 1" };
            var created = SampleTask();

            var svc = ServiceMock();
            svc.Setup(s => s.CreateTaskAsync(projectId, req))
               .Returns(Task.FromResult(created));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.CreateTask(projectId, req))
                .ShouldReturn()
                .Created(createdAt => createdAt
                    .AtAction(nameof(ProjectAssignmentsController.GetTaskById))
                    .Passing(result =>
                    {
                        var r = Assert.IsType<CreatedAtActionResult>(result);
                        Assert.Equal(projectId, r.RouteValues["projectId"]);
                        Assert.Equal(created.Id, r.RouteValues["taskId"]);
                        Assert.Same(created, r.Value);
                    }));

            svc.VerifyAll();
        }

        [Fact]
        public void GetTasks_Should_Return_Ok_With_List()
        {
            var projectId = Guid.NewGuid();
            var list = new List<AssignmentTaskResponse> { SampleTask(), SampleTask() };

            var svc = ServiceMock();
            svc.Setup(s => s.GetTasksAsync(projectId))
               .Returns(Task.FromResult<IEnumerable<AssignmentTaskResponse>>(list));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.GetTasks(projectId))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(list));

            svc.VerifyAll();
        }

        [Fact]
        public void GetTaskById_Should_Return_Ok_With_Model()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var task = SampleTask(taskId);

            var svc = ServiceMock();
            svc.Setup(s => s.GetTaskByIdAsync(projectId, taskId))
               .Returns(Task.FromResult(task));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.GetTaskById(projectId, taskId))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(task));

            svc.VerifyAll();
        }

        [Fact]
        public void UpdateTask_Should_Return_Ok_With_Model()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var req = new UpdateAssignmentTaskRequest { Title = "T2" };
            var updated = SampleTask(taskId);

            var svc = ServiceMock();
            svc.Setup(s => s.UpdateTaskAsync(projectId, taskId, req))
               .Returns(Task.FromResult(updated));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.UpdateTask(projectId, taskId, req))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(updated));

            svc.VerifyAll();
        }

        [Fact]
        public void DeleteTask_Should_Return_NoContent()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var svc = ServiceMock();
            svc.Setup(s => s.DeleteTaskAsync(projectId, taskId))
               .Returns(Task.CompletedTask);

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.DeleteTask(projectId, taskId))
                .ShouldReturn()
                .NoContent();

            svc.VerifyAll();
        }

        [Fact]
        public void CompleteTask_Should_Return_Ok_With_Service_Result()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var result = new AssignmentTaskResponse
            {
                Id = taskId,
                Title = "Task 1",
                Description = "Done",
                IsCompleted = true
            };

            var svc = ServiceMock();
            svc.Setup(s => s.CompleteTaskAsync(projectId, taskId))
               .Returns(Task.FromResult(result));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.CompleteTask(projectId, taskId))
                .ShouldReturn()
                .Ok(ok => ok.WithModel(result));

            svc.VerifyAll();
        }


        // -----------------------------
        // Search endpoint
        // -----------------------------

        [Fact]
        public void Search_Should_Return_Enumerable()
        {
            var req = new SearchProjectAssignmentRequest
            {
                Title = "Sample",
                Level = ProjectAssignmentLevel.Intermediate,  // <-- CORRECT
                CompanyName = "ACME",
                CompanySector = "IT",
                ProjectSkills = new List<string> { "C#" }
            };

            IEnumerable<ProjectAssignmentResponse> expected = new List<ProjectAssignmentResponse> { SampleProject() };

            var svc = ServiceMock();

            svc.Setup(s => s.SearchAsync(
                    It.IsAny<SpecOfProject>(),
                    1,
                    10,
                    It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult(expected));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.Search(req, 1, 10, default))
                .ShouldReturn()
                .ResultOfType<IEnumerable<ProjectAssignmentResponse>>();

            svc.VerifyAll();
        }

        // -----------------------------
        // Doc note: attribute vs actual
        // -----------------------------

        [Fact]
        public void CompleteTask_ProducesAttribute_DiffersFromActual()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var result = new AssignmentTaskResponse
            {
                Id = taskId,
                Title = "Task 1",
                Description = "Done",
                IsCompleted = true
            };

            var svc = ServiceMock();
            svc.Setup(s => s.CompleteTaskAsync(projectId, taskId))
               .Returns(Task.FromResult(result));

            MyController<ProjectAssignmentsController>
                .Instance(i => i.WithDependencies(svc.Object))
                .Calling(c => c.CompleteTask(projectId, taskId))
                .ShouldReturn()
                .Ok();

            svc.VerifyAll();
        }
    }
}
