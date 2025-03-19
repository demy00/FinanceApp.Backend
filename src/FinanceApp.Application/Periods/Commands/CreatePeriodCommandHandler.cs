using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Entities;
using MediatR;

namespace FinanceApp.Application.Periods.Commands;

public sealed class CreatePeriodCommandHandler : IRequestHandler<CreatePeriodCommand>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePeriodCommandHandler(IPeriodRepository periodRepository, IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreatePeriodCommand request, CancellationToken cancellationToken)
    {
        var period = new Period(request.Name, request.Description, request.StartDate, request.EndDate, request.UserId);

        _periodRepository.Add(period);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
