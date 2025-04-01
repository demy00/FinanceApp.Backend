using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using MediatR;

namespace FinanceApp.Application.Periods.Queries;

public sealed class GetPeriodsQueryHandler : IRequestHandler<GetPeriodsQuery, PageList<PeriodResponse>>
{
    private readonly IPeriodRepository _periodRepository;

    public GetPeriodsQueryHandler(IPeriodRepository periodRepository)
    {
        _periodRepository = periodRepository;
    }

    public async Task<PageList<PeriodResponse>> Handle(GetPeriodsQuery request, CancellationToken cancellationToken)
    {
        var periods = await _periodRepository
            .GetAsync(
            request.UserId,
            request.SearchTerm,
            request.SortColumn,
            request.SortOrder,
            request.Page,
            request.PageSize,
            cancellationToken);

        return periods;
    }
}
