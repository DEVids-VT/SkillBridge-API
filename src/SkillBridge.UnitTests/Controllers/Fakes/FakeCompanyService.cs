using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkillBridge.Models.Request;   
using SkillBridge.Models.Response;  
using SkillBridge.Services.Company; 

namespace SkillBridge.UnitTests.Controllers.Fakes
{
    public sealed class FakeCompanyService : ICompanyService
    {
        public Task<CompanyResponse> CreateAsync(CreateCompanyRequest request)
            => Task.FromResult(new CompanyResponse { Id = Guid.NewGuid(), Name = request.Name ?? "Fake" });

        public Task<CompanyResponse> GetByIdAsync(Guid id)
            => Task.FromResult(new CompanyResponse { Id = id, Name = "Fake" });

        public Task<CompanyResponse> GetMyCompanyAsync(string? userId)
            => Task.FromResult(new CompanyResponse { Id = Guid.NewGuid(), Name = userId ?? "Fake" });

        public Task<IEnumerable<CompanyResponse>> GetAllAsync()
            => Task.FromResult<IEnumerable<CompanyResponse>>(Array.Empty<CompanyResponse>());

        public Task<CompanyResponse> UpdateAsync(Guid id, UpdateCompanyRequest request)
            => Task.FromResult(new CompanyResponse { Id = id, Name = request.Name ?? "Fake" });

        public Task DeleteAsync(Guid id) => Task.CompletedTask;

        public Task<CompanyResponse> UpdateCompanyLogoAsync(Guid id, UpdateCompanyLogoRequest request)
            => Task.FromResult(new CompanyResponse { Id = id, Name = "Fake" });
    }
}
