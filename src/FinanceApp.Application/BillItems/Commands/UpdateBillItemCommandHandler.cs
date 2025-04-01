using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.BillItems.Commands;

public sealed class UpdateBillItemCommandHandler : IRequestHandler<UpdateBillItemCommand>
{
    private readonly IBillItemRepository _billItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBillItemCommandHandler(IBillItemRepository billItemRepository, IUnitOfWork unitOfWork)
    {
        _billItemRepository = billItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateBillItemCommand request, CancellationToken cancellationToken)
    {
        var billItem = await _billItemRepository.GetDomainByIdAsync(request.Id, request.UserId, cancellationToken);

        if (billItem is null)
        {
            throw new BillItemNotFoundException(request.Id);
        }

        billItem.Update(request.Name, request.Description, request.Category, request.Price, request.Quantity);

        _billItemRepository.Update(billItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
