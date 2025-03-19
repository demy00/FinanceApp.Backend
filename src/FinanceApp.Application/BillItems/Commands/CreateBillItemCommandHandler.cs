using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Entities;
using MediatR;

namespace FinanceApp.Application.BillItems.Commands;

public sealed class CreateBillItemCommandHandler : IRequestHandler<CreateBillItemCommand>
{
    private readonly IBillItemRepository _billItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBillItemCommandHandler(IBillItemRepository billItemRepository, IUnitOfWork unitOfWork)
    {
        _billItemRepository = billItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateBillItemCommand request, CancellationToken cancellationToken)
    {
        var billItem = new BillItem(
            request.Name,
            request.Description,
            request.Category,
            request.Price,
            request.Quantity,
            request.UserId);

        _billItemRepository.Add(billItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
