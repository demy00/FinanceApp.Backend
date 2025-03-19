using FluentValidation;

namespace FinanceApp.Application.BillItems.Queries;

public class GetBillItemsQueryValidator : AbstractValidator<GetBillItemsQuery>
{
    public GetBillItemsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId is required.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

        RuleFor(x => x.SortOrder)
            .Must(order => order == null ||
                           order.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                           order.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortOrder must be either 'asc' or 'desc'.");
    }
}
