using MediatR;

namespace FinanceApp.Application.Periods.Commands;

public record DeletePeriodCommand(Guid Id, Guid UserId) : IRequest;
