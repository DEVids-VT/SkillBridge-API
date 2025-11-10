// TestsBootstrapper.cs

using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyTested.AspNetCore.Mvc;
using SkillBridge.Services.ProjectAssignment;

namespace SkillBridge.UnitTests.Controllers.ProjectAssignmentControllerTests;

public sealed class TestsBootstrapper
{
    public TestsBootstrapper()
    {
        MyApplication
            .StartsFrom<Program>() // or .StartsFrom<Startup>()
            .WithServices(services =>
            {
                var svcMock = new Mock<IProjectAssignmentService>(MockBehavior.Loose);
                services.AddSingleton<IProjectAssignmentService>(svcMock.Object);
            });
    }
}