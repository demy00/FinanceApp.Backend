using FluentValidation;

namespace FinanceApp.Application.Periods.Commands;

public class DeletePeriodCommandValidator : AbstractValidator<DeletePeriodCommand>
{
    public DeletePeriodCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Period Id is required.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
