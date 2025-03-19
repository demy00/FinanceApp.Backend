namespace FinanceApp.Domain.Exceptions;

public sealed class CategoryNotFoundException : FinanceAppDomainException
{
    public CategoryNotFoundException(Guid id)
        : base($"The category with ID '{id}' was not found.") { }
}
