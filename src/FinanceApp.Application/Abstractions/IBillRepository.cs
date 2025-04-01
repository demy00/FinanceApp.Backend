using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Domain.Entities;

namespace FinanceApp.Application.Abstractions;

public interface IBillRepository
{
    Task<Bill?> GetDomainByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<BillResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<PageList<BillResponse>> GetAsync(
        Guid userId,
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    void Add(Bill bill);

    void Update(Bill bill);

    void Remove(Bill bill);
}
