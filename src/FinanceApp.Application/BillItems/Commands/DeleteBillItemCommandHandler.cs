using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.BillItems.Commands;

public sealed class DeleteBillItemCommandHandler : IRequestHandler<DeleteBillItemCommand>
{
    private readonly IBillItemRepository _billItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBillItemCommandHandler(IBillItemRepository billItemRepository, IUnitOfWork unitOfWork)
    {
        _billItemRepository = billItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteBillItemCommand request, CancellationToken cancellationToken)
    {
        var billItem = await _billItemRepository.GetDomainByIdAsync(request.Id, request.UserId, cancellationToken);

        if (billItem is null)
        {
            throw new BillItemNotFoundException(request.Id);
        }

        _billItemRepository.Remove(billItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
