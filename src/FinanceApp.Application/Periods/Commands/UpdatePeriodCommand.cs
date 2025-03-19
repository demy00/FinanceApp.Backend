using MediatR;

namespace FinanceApp.Application.Periods.Commands;

public record UpdatePeriodCommand(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid UserId) : IRequest;
