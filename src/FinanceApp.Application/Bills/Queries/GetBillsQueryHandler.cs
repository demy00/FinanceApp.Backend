using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using MediatR;

namespace FinanceApp.Application.Bills.Queries;

public sealed class GetBillsQueryHandler : IRequestHandler<GetBillsQuery, PageList<BillResponse>>
{
    private readonly IBillRepository _billRepository;

    public GetBillsQueryHandler(IBillRepository billRepository)
    {
        _billRepository = billRepository;
    }

    public async Task<PageList<BillResponse>> Handle(GetBillsQuery request, CancellationToken cancellationToken)
    {
        var bills = await _billRepository.GetAsync(
            request.UserId,
            request.SearchTerm,
            request.SortColumn,
            request.SortOrder,
            request.Page,
            request.PageSize,
            cancellationToken);

        return bills;
    }
}
