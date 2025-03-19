using FluentValidation;

namespace FinanceApp.Application.BillItems.Commands;

public class UpdateBillItemCommandValidator : AbstractValidator<UpdateBillItemCommand>
{
    public UpdateBillItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Bill item Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Bill item name is required.")
            .MaximumLength(100).WithMessage("Bill item name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Category)
            .NotNull().WithMessage("Category must be provided.");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Price must be provided.")
            .Must(price => price.Amount > 0)
                .WithMessage("Price must be greater than zero.")
            .Must(price => !string.IsNullOrWhiteSpace(price.Currency))
                .WithMessage("Currency must be provided.");

        RuleFor(x => x.Quantity)
            .NotNull().WithMessage("Quantity must be provided.")
            .Must(quantity => quantity.Value > 0)
                .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
