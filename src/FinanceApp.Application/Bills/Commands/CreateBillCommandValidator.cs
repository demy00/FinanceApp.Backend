using FluentValidation;

namespace FinanceApp.Application.Bills.Commands;

public class CreateBillCommandValidator : AbstractValidator<CreateBillCommand>
{
    public CreateBillCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Bill name is required.")
            .MaximumLength(100).WithMessage("Bill name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
