using FluentValidation;

namespace FinanceApp.Application.Periods.Queries;

public class GetPeriodQueryValidator : AbstractValidator<GetPeriodQuery>
{
    public GetPeriodQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Period Id is required.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
