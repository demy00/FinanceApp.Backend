using MediatR;

namespace FinanceApp.Application.Bills.Commands;

public record UpdateBillCommand(
    Guid Id,
    string Name,
    string Description,
    Guid UserId) : IRequest;
