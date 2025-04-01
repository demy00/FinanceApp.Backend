using FluentValidation;

namespace FinanceApp.Application.Bills.Queries;

public class GetBillQueryValidator : AbstractValidator<GetBillQuery>
{
    public GetBillQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Bill Id is required.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
