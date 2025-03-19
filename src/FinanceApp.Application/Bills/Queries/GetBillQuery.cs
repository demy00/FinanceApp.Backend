using FinanceApp.Application.DTOs;
using MediatR;

namespace FinanceApp.Application.Bills.Queries;

public record GetBillQuery(Guid Id, Guid UserId) : IRequest<BillResponse>;
