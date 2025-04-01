using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Domain.Entities;

namespace FinanceApp.Application.Abstractions;

public interface IPeriodRepository
{
    Task<Period?> GetDomainByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<PeriodResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<PageList<PeriodResponse>> GetAsync(
        Guid userId,
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    void Add(Period period);

    void Update(Period period);

    void Remove(Period period);
}
