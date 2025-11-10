using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SkillBridge.Services.CurrentUser;

namespace SkillBridge.UnitTests.Controllers.Fakes
{
    public sealed class FakeCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _http;

        public FakeCurrentUser(IHttpContextAccessor http) => _http = http;

        public string GetUserId()
            => _http.HttpContext?.User?
                   .FindFirst("sub")?.Value // common for JWTs
               ?? _http.HttpContext?.User?
                   .FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? "test-user";

        public bool IsInRole(string role)
            => _http.HttpContext?.User?.IsInRole(role) ?? false;

        public IEnumerable<Claim> GetClaims()
            => _http.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();
    }
}
