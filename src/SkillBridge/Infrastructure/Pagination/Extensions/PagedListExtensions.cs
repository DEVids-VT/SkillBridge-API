using SkillBridge.Infrastructure.Pagination.Abstractions;

namespace SkillBridge.Infrastructure.Pagination.Extensions
{
    public static class PagedListExtensions
    {
        public static IPage<T> ToPage<T>(this IPagedList<T> pagedList) => new Page<T>(
            pagedList.Items,
            pagedList.PageIndex,
            pagedList.PageSize,
            (int)pagedList.TotalCount);
    }
}
