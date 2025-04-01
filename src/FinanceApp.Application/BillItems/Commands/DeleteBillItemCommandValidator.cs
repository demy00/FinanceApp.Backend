using FluentValidation;

namespace FinanceApp.Application.BillItems.Commands;

public class DeleteBillItemCommandValidator : AbstractValidator<DeleteBillItemCommand>
{
    public DeleteBillItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Bill item Id is required.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
