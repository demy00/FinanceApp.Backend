using FinanceApp.Application.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<PageList<T>> ToPageListAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PageList<T>(items, page, pageSize, totalCount);
    }
}
