namespace SkillBridge.Infrastructure.Pagination.Headers
{
    using SkillBridge.Infrastructure.Pagination.Abstractions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using SkillBridge.Infrastructure.Pagination.Extensions;

    public class PaginationHeadersFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(
            ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult
                && objectResult.Value is IPage page)
            {
                context.HttpContext.Response.Headers.AddPaginationHeader(
                    page.CurrentPage,
                    page.PageSize,
                    page.TotalCount);
            }

            await next();
        }
    }
}
