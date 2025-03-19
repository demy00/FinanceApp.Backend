using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Domain.Entities;

namespace FinanceApp.Application.Abstractions;

public interface IBillItemRepository
{
    Task<BillItem?> GetDomainByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<BillItemResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<PageList<BillItemResponse>> GetAsync(
        Guid userId,
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    void Add(BillItem billItem);

    void Update(BillItem billItem);

    void Remove(BillItem billItem);
}

