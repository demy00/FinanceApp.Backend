using FinanceApp.Application.Abstractions;
using MediatR;

namespace FinanceApp.Application.Bills.Commands;

public sealed class CreateBillCommandHandler : IRequestHandler<CreateBillCommand>
{
    private readonly IBillRepository _billRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBillCommandHandler(IBillRepository billRepository, IUnitOfWork unitOfWork)
    {
        _billRepository = billRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateBillCommand request, CancellationToken cancellationToken)
    {
        var bill = new Domain.Entities.Bill(request.Name, request.Description, request.UserId);
        _billRepository.Add(bill);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
