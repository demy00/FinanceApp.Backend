using FluentValidation;

namespace FinanceApp.Application.Bills.Commands;

public class DeleteBillCommandValidator : AbstractValidator<DeleteBillCommand>
{
    public DeleteBillCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Bill Id is required.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
