using FinanceApp.Application.DTOs;
using MediatR;

namespace FinanceApp.Application.Periods.Queries;

public record GetPeriodQuery(Guid Id, Guid UserId) : IRequest<PeriodResponse>;
