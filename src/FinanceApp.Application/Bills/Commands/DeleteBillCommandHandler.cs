using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Bills.Commands;

public sealed class DeleteBillCommandHandler : IRequestHandler<DeleteBillCommand>
{
    private readonly IBillRepository _billRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBillCommandHandler(IBillRepository billRepository, IUnitOfWork unitOfWork)
    {
        _billRepository = billRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteBillCommand request, CancellationToken cancellationToken)
    {
        var bill = await _billRepository.GetDomainByIdAsync(request.Id, request.UserId, cancellationToken);

        if (bill is null)
        {
            throw new BillNotFoundException(request.Id);
        }

        _billRepository.Remove(bill);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
