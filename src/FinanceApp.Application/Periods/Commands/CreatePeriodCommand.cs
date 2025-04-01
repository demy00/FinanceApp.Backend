using MediatR;

namespace FinanceApp.Application.Periods.Commands;

public record CreatePeriodCommand(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid UserId) : IRequest;
