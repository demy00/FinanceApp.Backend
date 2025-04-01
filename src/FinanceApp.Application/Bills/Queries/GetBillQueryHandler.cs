using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Bills.Queries;

public sealed class GetBillQueryHandler : IRequestHandler<GetBillQuery, BillResponse>
{
    private readonly IBillRepository _billRepository;

    public GetBillQueryHandler(IBillRepository billRepository)
    {
        _billRepository = billRepository;
    }

    public async Task<BillResponse> Handle(GetBillQuery request, CancellationToken cancellationToken)
    {
        var bill = await _billRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);

        if (bill is null)
        {
            throw new BillNotFoundException(request.Id);
        }

        return bill;
    }
}
