using DailyWorkReport.ViewModels.Paging;
using Microsoft.EntityFrameworkCore;

namespace DailyWorkReport.Extensions;

public static class QueryableExtensions
{
    public static async Task<(List<T> Items, PagingInfo Paging)> ToPagedAsync<T>(this IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var totalPages = Math.Max(1, (totalCount + pageSize -1) / pageSize);
        var current = Math.Clamp(page, 1, totalPages); 

        var items = await query
            .Skip((current - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, new PagingInfo
        {
            Page = current,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }
}
