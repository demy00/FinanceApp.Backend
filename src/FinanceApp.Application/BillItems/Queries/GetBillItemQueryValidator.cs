using FluentValidation;

namespace FinanceApp.Application.BillItems.Queries;

public class GetBillItemQueryValidator : AbstractValidator<GetBillItemQuery>
{
    public GetBillItemQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Bill item Id is required.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
