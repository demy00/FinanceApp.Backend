using MediatR;

namespace FinanceApp.Application.BillItems.Commands;

public record DeleteBillItemCommand(Guid Id, Guid UserId) : IRequest;
