using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using MediatR;

namespace FinanceApp.Application.BillItems.Queries;

public sealed class GetBillItemsQueryHandler : IRequestHandler<GetBillItemsQuery, PageList<BillItemResponse>>
{
    private readonly IBillItemRepository _billItemRepository;

    public GetBillItemsQueryHandler(IBillItemRepository billItemRepository)
    {
        _billItemRepository = billItemRepository;
    }

    public async Task<PageList<BillItemResponse>> Handle(GetBillItemsQuery request, CancellationToken cancellationToken)
    {
        var billItems = await _billItemRepository.GetAsync(
            request.UserId,
            request.SearchTerm,
            request.SortColumn,
            request.SortOrder,
            request.Page,
            request.PageSize,
            cancellationToken);

        return billItems;
    }
}

