using FluentValidation;

namespace FinanceApp.Application.Periods.Commands;

public class CreatePeriodCommandValidator : AbstractValidator<CreatePeriodCommand>
{
    public CreatePeriodCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Period name is required.")
            .MaximumLength(100).WithMessage("Period name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("StartDate must be a valid date.");

        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("EndDate must be a valid date.")
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be after StartDate.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");
    }
}
