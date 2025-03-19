using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Bills.Commands;

public sealed class UpdateBillCommandHandler : IRequestHandler<UpdateBillCommand>
{
    private readonly IBillRepository _billRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBillCommandHandler(IBillRepository billRepository, IUnitOfWork unitOfWork)
    {
        _billRepository = billRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateBillCommand request, CancellationToken cancellationToken)
    {
        var bill = await _billRepository.GetDomainByIdAsync(request.Id, request.UserId, cancellationToken);

        if (bill is null)
        {
            throw new BillNotFoundException(request.Id);
        }

        bill.Update(request.Name, request.Description);

        _billRepository.Update(bill);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
