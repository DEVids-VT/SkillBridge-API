using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkillBridge.Models.Request;
using SkillBridge.Models.Response;
using SkillBridge.Services.Company;
using SkillBridge.Services.CurrentUser;
using SkillBridge.UnitTests.Controllers.Fakes;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

// IMPORTANT: no namespace here. Class must be public and named exactly TestStartup.
public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Load your API controllers into the test host
        services.AddControllers()
                .AddApplicationPart(typeof(SkillBridge.Controllers.CompaniesController).Assembly);

        // Minimal auth so Challenge/Forbid work in pipeline tests
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Test";
            options.DefaultChallengeScheme = "Test";
            options.DefaultScheme = "Test";
        })
       .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

        // 👉 Define the policies your controller uses
        services.AddAuthorization(options =>
        {
            options.AddPolicy("CompanyScope", p =>
                p.RequireAuthenticatedUser()
                 .RequireClaim("scope", "CompanyScope"));

            options.AddPolicy("CompaniesScope", p =>
                p.RequireAuthenticatedUser()
                 .RequireClaim("scope", "CompaniesScope"));

            // If your real "Company" policy is role-based, switch to RequireRole("Company")
            options.AddPolicy("Company", p =>
                p.RequireAuthenticatedUser()
                 .RequireClaim("scope", "Company"));

            options.AddPolicy("Candidate", p => 
            p.RequireAuthenticatedUser()
            .RequireClaim("scope", "Candidate"));

            options.AddPolicy("CandidateScope", p =>
           p.RequireAuthenticatedUser()
           .RequireClaim("scope", "CandidateScope"));
        });

        services.AddHttpContextAccessor();

        services.AddScoped<ICompanyService, FakeCompanyService>();
        services.AddScoped<ICurrentUser, FakeCurrentUser>();

        //services.Configure<ApiBehaviorOptions>(o =>
        //{
        //    o.SuppressModelStateInvalidFilter = true;
        //});

        //services.Configure<MvcOptions>(o =>
        //{
        //    o.Filters.Remove(new UnsupportedContentTypeFilter());
        //});
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}

// Simple auth handler: if tests set a user (WithUser(...)), authenticate; otherwise NoResult → 401 Challenge.
public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                           ILoggerFactory logger,
                           UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // If the test does NOT ask to authenticate → Challenge (401)
        if (!Request.Headers.TryGetValue("X-Test-Auth", out var auth) || auth != "1")
            return Task.FromResult(AuthenticateResult.NoResult());

        // Build an authenticated identity
        var identity = new ClaimsIdentity("Test");

        // Optional scope (policy) coming from header; omit to trigger Forbid
        if (Request.Headers.TryGetValue("X-Test-Scope", out var scope) && !string.IsNullOrWhiteSpace(scope))
            identity.AddClaim(new Claim("scope", scope!));

        // Optional role (for [Authorize(Policy = "Company")] if you use RequireRole)
        if (Request.Headers.TryGetValue("X-Test-Role", out var role) && !string.IsNullOrWhiteSpace(role))
            identity.AddClaim(new Claim(ClaimTypes.Role, role!));

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}


