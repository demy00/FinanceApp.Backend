using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Domain.Entities;
using FinanceApp.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceApp.Infrastructure.Repositories;

public class BillRepository : IBillRepository
{
    private readonly ApplicationDbContext _context;

    public BillRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Bill bill)
    {
        _context.Bills.Add(bill);
    }

    public Task<Bill?> GetDomainByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return _context.Bills
            .Include(b => b.BillItems)
            .SingleOrDefaultAsync(b => b.Id == id && b.UserId == userId, cancellationToken);
    }

    public Task<BillResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return _context.Bills
            .Where(b => b.Id == id && b.UserId == userId)
            .Select(b => new BillResponse(
                b.Id,
                b.Name,
                b.Description,
                new MoneyDto(
                    b.BillItems.Sum(bi => bi.Price.Amount),
                    b.BillItems.Select(bi => bi.Price.Currency).FirstOrDefault() ?? "UNKNOWN")
                ))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PageList<BillResponse>> GetAsync(
        Guid userId,
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Bill> baseQuery = _context.Bills
            .Where(b => b.UserId == userId);

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

        IQueryable<BillResponse> responseQuery = baseQuery
           .Select(b => new BillResponse(
               b.Id,
               b.Name,
               b.Description,
               new MoneyDto(
                   b.BillItems.Sum(bi => bi.Price.Amount),
                   b.BillItems.Select(bi => bi.Price.Currency).FirstOrDefault() ?? "UNKNOWN")
               ));

        var periodsResponse = await responseQuery
            .ToPageListAsync(page, pageSize, cancellationToken);

        return periodsResponse;
    }

    private static IQueryable<Bill> ApplySearchTermToQuery(string searchTerm, IQueryable<Bill> baseQuery)
    {
        if (searchTerm.StartsWith("currency_", StringComparison.OrdinalIgnoreCase))
        {
            var currencyCode = searchTerm.Substring("currency_".Length).Trim().ToUpperInvariant();

            return baseQuery.Where(b =>
                b.BillItems.Any(bi => bi.Price.Currency == currencyCode));
        }

        return baseQuery.Where(b =>
            b.Name.Contains(searchTerm) ||
            b.Description.Contains(searchTerm));
    }

    private static Expression<Func<Bill, object>> GetSortKeySelector(string? sortColumn)
    {
        if (string.IsNullOrWhiteSpace(sortColumn))
        {
            return b => b.Id;
        }

        sortColumn = sortColumn.ToLower();

        if (sortColumn.StartsWith("amount"))
        {
            var parts = sortColumn.Split('_');
            if (parts.Length == 2)
            {
                string currency = parts[1].ToUpperInvariant();
                return b => b.BillItems
                    .Where(bi => bi.Price.Currency == currency)
                    .Sum(bi => bi.Price.Amount);
            }
            else
            {
                return b => b.BillItems.Sum(bi => bi.Price.Amount);
            }
        }

        return b => b.Name;
    }

    public void Remove(Bill bill)
    {
        _context.Bills.Remove(bill);
    }

    public void Update(Bill bill)
    {
        _context.Bills.Update(bill);
    }
}
