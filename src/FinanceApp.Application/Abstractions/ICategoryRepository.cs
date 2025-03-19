using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Domain.Entities;

namespace FinanceApp.Application.Abstractions;

public interface ICategoryRepository
{
    Task<Category?> GetDomainByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<CategoryResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<PageList<CategoryResponse>> GetAsync(
        Guid userId,
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    void Add(Category category);

    void Update(Category category);

    void Remove(Category category);
}

