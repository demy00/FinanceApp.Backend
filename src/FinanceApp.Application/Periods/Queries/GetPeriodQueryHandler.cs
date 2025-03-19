using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Periods.Queries;

public sealed class GetPeriodQueryHandler : IRequestHandler<GetPeriodQuery, PeriodResponse>
{
    private readonly IPeriodRepository _periodRepository;

    public GetPeriodQueryHandler(IPeriodRepository periodRepository)
    {
        _periodRepository = periodRepository;
    }

    public async Task<PeriodResponse> Handle(GetPeriodQuery request, CancellationToken cancellationToken)
    {
        var periodResponse = await _periodRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);

        if (periodResponse is null)
        {
            throw new PeriodNotFoundException(request.Id);
        }

        return periodResponse;
    }
}
