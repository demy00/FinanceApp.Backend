using MediatR;

namespace FinanceApp.Application.Bills.Commands;

public record CreateBillCommand(
    string Name,
    string Description,
    Guid UserId) : IRequest;
