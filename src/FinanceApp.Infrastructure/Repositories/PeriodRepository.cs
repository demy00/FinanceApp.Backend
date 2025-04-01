using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Domain.Entities;
using FinanceApp.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceApp.Infrastructure.Repositories;

public class PeriodRepository : IPeriodRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public PeriodRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public void Add(Period period)
    {
        _applicationDbContext.Periods.Add(period);
    }

    public async Task<Period?> GetDomainByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.Periods
            .Include(p => p.Bills)
                .ThenInclude(b => b.BillItems)
            .SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId, cancellationToken);
    }

    public async Task<PeriodResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var periodResponse = await _applicationDbContext.Periods
            .Where(p => p.Id == id && p.UserId == userId)
            .Select(p => new PeriodResponse(
                p.Id,
                p.Name,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Bills
                    .SelectMany(b => b.BillItems)
                    .GroupBy(bi => bi.Price.Currency)
                    .Select(g => new MoneyDto(g.Sum(bi => bi.Price.Amount), g.Key))
            .ToList()))
            .SingleOrDefaultAsync(cancellationToken);

        return periodResponse;
    }

    public async Task<PageList<PeriodResponse>> GetAsync(
        Guid userId,
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Period> baseQuery = _applicationDbContext.Periods
            .Where(p => p.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            baseQuery = ApplySearchTermToQuery(searchTerm, baseQuery);
        }

        if (sortOrder?.ToLower() == "desc")
        {
            baseQuery = baseQuery.OrderByDescending(GetSortKeySelector(sortColumn));
        }
        else
        {
            baseQuery = baseQuery.OrderBy(GetSortKeySelector(sortColumn));
        }

        IQueryable<PeriodResponse> responseQuery = baseQuery
           .Select(p => new PeriodResponse(
               p.Id,
               p.Name,
               p.Description,
               p.StartDate,
               p.EndDate,
               p.Bills
                   .SelectMany(b => b.BillItems)
                   .GroupBy(bi => bi.Price.Currency)
                   .Select(g => new MoneyDto(g.Sum(bi => bi.Price.Amount), g.Key))
           .ToList()));

        var periodsResponse = await responseQuery
            .ToPageListAsync(page, pageSize, cancellationToken);

        return periodsResponse;
    }

    private static IQueryable<Period> ApplySearchTermToQuery(string searchTerm, IQueryable<Period> baseQuery)
    {
        if (searchTerm.StartsWith("currency_", StringComparison.OrdinalIgnoreCase))
        {
            var currencyCode = searchTerm.Substring("currency_".Length).Trim().ToUpperInvariant();

            return baseQuery.Where(p =>
                p.Bills.SelectMany(b => b.BillItems).Any(bi => bi.Price.Currency == currencyCode));
        }

        return baseQuery.Where(p =>
            p.Name.Contains(searchTerm) ||
            p.Description.Contains(searchTerm));
    }

    private static Expression<Func<Period, object>> GetSortKeySelector(string? sortColumn)
    {
        if (string.IsNullOrWhiteSpace(sortColumn))
        {
            return p => p.Id;
        }

        sortColumn = sortColumn.ToLower();

        if (sortColumn.StartsWith("amount"))
        {
            var parts = sortColumn.Split('_');
            if (parts.Length == 2)
            {
                string currency = parts[1].ToUpperInvariant();
                return p => p.Bills
                    .SelectMany(b => b.BillItems)
                    .Where(bi => bi.Price.Currency == currency)
                    .Sum(bi => bi.Price.Amount);
            }
            else
            {
                return p => p.Bills.SelectMany(b => b.BillItems).Sum(bi => bi.Price.Amount);
            }
        }

        return sortColumn switch
        {
            "name" => p => p.Name,
            "startdate" => p => p.StartDate,
            "enddate" => p => p.EndDate,
            _ => p => p.Id
        };
    }

    public void Remove(Period period)
    {
        _applicationDbContext.Periods.Remove(period);
    }

    public void Update(Period period)
    {
        _applicationDbContext.Periods.Update(period);
    }
}
