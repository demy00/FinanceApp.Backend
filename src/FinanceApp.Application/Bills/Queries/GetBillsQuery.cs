using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using MediatR;

namespace FinanceApp.Application.Bills.Queries;

public record GetBillsQuery(
    Guid UserId,
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder,
    int Page,
    int PageSize) : IRequest<PageList<BillResponse>>;
