using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Domain.Entities;
using FinanceApp.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceApp.Infrastructure.Repositories;

public class BillItemRepository : IBillItemRepository
{
    private readonly ApplicationDbContext _context;

    public BillItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(BillItem billItem)
    {
        _context.BillItems.Add(billItem);
    }

    public Task<BillItem?> GetDomainByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return _context.BillItems
            .SingleOrDefaultAsync(bi => bi.Id == id && bi.UserId == userId, cancellationToken);
    }

    public Task<BillItemResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return _context.BillItems
            .Where(bi => bi.Id == id && bi.UserId == userId)
            .Select(bi => new BillItemResponse(
                bi.Id,
                bi.Name,
                bi.Description,
               new CategoryDto(bi.Category.Name, bi.Category.Description),
               new MoneyDto(bi.Price.Amount, bi.Price.Currency),
               new QuantityDto(bi.Quantity.Value)))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PageList<BillItemResponse>> GetAsync(
        Guid userId,
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<BillItem> baseQuery = _context.BillItems
            .Where(item => item.UserId == userId);

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

        IQueryable<BillItemResponse> responseQuery = baseQuery
           .Select(bi => new BillItemResponse(
               bi.Id,
               bi.Name,
               bi.Description,
               new CategoryDto(bi.Category.Name, bi.Category.Description),
               new MoneyDto(bi.Price.Amount, bi.Price.Currency),
               new QuantityDto(bi.Quantity.Value)
               ));

        var billItemsResponse = await responseQuery
            .ToPageListAsync(page, pageSize, cancellationToken);

        return billItemsResponse;
    }

    private static IQueryable<BillItem> ApplySearchTermToQuery(string searchTerm, IQueryable<BillItem> baseQuery)
    {
        if (searchTerm.StartsWith("currency_", StringComparison.OrdinalIgnoreCase))
        {
            var currencyCode = searchTerm.Substring("currency_".Length).Trim().ToUpperInvariant();

            return baseQuery.Where(bi => bi.Price.Currency == currencyCode);
        }

        return baseQuery.Where(bi =>
            bi.Name.Contains(searchTerm) ||
            bi.Description.Contains(searchTerm));
    }

    private static Expression<Func<BillItem, object>> GetSortKeySelector(string? sortColumn)
    {
        return sortColumn switch
        {
            "name" => bi => bi.Name,
            "category" => bi => bi.Category.Name,
            "currency" => bi => bi.Price.Currency,
            "amount" => bi => bi.Price.Amount,
            "quantity" => bi => bi.Quantity,
            _ => bi => bi.Id
        };
    }

    public void Remove(BillItem billItem)
    {
        _context.BillItems.Remove(billItem);
    }

    public void Update(BillItem billItem)
    {
        _context.BillItems.Update(billItem);
    }
}
