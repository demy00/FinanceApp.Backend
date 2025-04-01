using MediatR;

namespace FinanceApp.Application.Bills.Commands;

public record DeleteBillCommand(Guid Id, Guid UserId) : IRequest;
