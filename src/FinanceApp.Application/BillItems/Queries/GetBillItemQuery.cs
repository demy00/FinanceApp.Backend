using FinanceApp.Application.DTOs;
using MediatR;

namespace FinanceApp.Application.BillItems.Queries;

public record GetBillItemQuery(Guid Id, Guid UserId) : IRequest<BillItemResponse>;
