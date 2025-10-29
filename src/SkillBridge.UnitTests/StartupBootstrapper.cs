using System.Runtime.CompilerServices;
using MyTested.AspNetCore.Mvc;

// Runs once when the test assembly loads:
public static class StartupBootstrapper
{
    [ModuleInitializer]
    public static void Init()
        => MyApplication.StartsFrom<TestStartup>();
}
