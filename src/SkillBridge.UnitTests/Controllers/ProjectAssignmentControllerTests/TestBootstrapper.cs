// TestsBootstrapper.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using MyTested.AspNetCore.Mvc;
using SkillBridge.Services.ProjectAssignment;

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