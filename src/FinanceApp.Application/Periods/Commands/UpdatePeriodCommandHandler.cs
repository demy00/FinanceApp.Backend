using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Periods.Commands;

public sealed class UpdatePeriodCommandHandler : IRequestHandler<UpdatePeriodCommand>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePeriodCommandHandler(IPeriodRepository periodRepository, IUnitOfWork unitOfWork)
    {
        _periodRepository = periodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdatePeriodCommand request, CancellationToken cancellationToken)
    {
        var period = await _periodRepository.GetDomainByIdAsync(request.Id, request.UserId, cancellationToken);

        if (period is null)
        {
            throw new PeriodNotFoundException(request.Id);
        }

        period.Update(request.Name, request.Description, request.StartDate, request.EndDate);

        _periodRepository.Update(period);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
