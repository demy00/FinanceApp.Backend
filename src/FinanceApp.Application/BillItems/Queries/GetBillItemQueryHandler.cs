using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.BillItems.Queries;

public sealed class GetBillItemQueryHandler : IRequestHandler<GetBillItemQuery, BillItemResponse>
{
    private readonly IBillItemRepository _billItemRepository;

    public GetBillItemQueryHandler(IBillItemRepository billItemRepository)
    {
        _billItemRepository = billItemRepository;
    }

    public async Task<BillItemResponse> Handle(GetBillItemQuery request, CancellationToken cancellationToken)
    {
        var billItem = await _billItemRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);

        if (billItem is null)
        {
            throw new BillItemNotFoundException(request.Id);
        }

        return new BillItemResponse(
            billItem.Id,
            billItem.Name,
            billItem.Description,
            billItem.Category,
            billItem.Price,
            billItem.Quantity);
    }
}
