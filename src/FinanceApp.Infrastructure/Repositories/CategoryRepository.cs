using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Domain.Entities;
using FinanceApp.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceApp.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Category category)
    {
        _context.Categories.Add(category);
    }

    public Task<Category?> GetDomainByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return _context.Categories
            .SingleOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);
    }

    public Task<CategoryResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return _context.Categories
            .Where(c => c.Id == id && c.UserId == userId)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Description))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PageList<CategoryResponse>> GetAsync(
        Guid userId,
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Category> baseQuery = _context.Categories
            .Where(c => c.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            baseQuery = baseQuery.Where(bi =>
                bi.Name.Contains(searchTerm) ||
                bi.Description.Contains(searchTerm));
        }

        if (sortOrder?.ToLower() == "desc")
        {
            baseQuery = baseQuery.OrderByDescending(GetSortKeySelector(sortColumn));
        }
        else
        {
            baseQuery = baseQuery.OrderBy(GetSortKeySelector(sortColumn));
        }

        IQueryable<CategoryResponse> responseQuery = baseQuery
           .Select(c => new CategoryResponse(
               c.Id,
               c.Name,
               c.Description
               ));

        var categoriesResponse = await responseQuery
            .ToPageListAsync(page, pageSize, cancellationToken);

        return categoriesResponse;
    }

    private static Expression<Func<Category, object>> GetSortKeySelector(string? sortColumn)
    {
        return sortColumn switch
        {
            "name" => c => c.Name,
            _ => c => c.Id
        };
    }

    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }
}
