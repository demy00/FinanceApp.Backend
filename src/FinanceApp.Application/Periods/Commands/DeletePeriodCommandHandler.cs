using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Periods.Commands;

public sealed class DeletePeriodCommandHandler : IRequestHandler<DeletePeriodCommand>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePeriodCommandHandler(IPeriodRepository periodRepository, IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await _periodRepository.GetDomainByIdAsync(request.Id, request.UserId, cancellationToken);

        if (period is null)
        {
            throw new PeriodNotFoundException(request.Id);
        }

        _periodRepository.Remove(period);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
