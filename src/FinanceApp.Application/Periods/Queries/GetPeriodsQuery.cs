using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using MediatR;

namespace FinanceApp.Application.Periods.Queries;

public record GetPeriodsQuery(
    Guid UserId,
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder,
    int Page,
    int PageSize) : IRequest<PageList<PeriodResponse>>;
