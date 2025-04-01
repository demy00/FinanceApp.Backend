using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using MediatR;

namespace FinanceApp.Application.BillItems.Queries;

public record GetBillItemsQuery(
    Guid UserId,
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder,
    int Page,
    int PageSize) : IRequest<PageList<BillItemResponse>>;