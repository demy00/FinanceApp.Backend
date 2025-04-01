namespace FinanceApp.Domain.Exceptions;

public sealed class BillItemNotFoundException : FinanceAppDomainException
{
    public BillItemNotFoundException(Guid id)
        : base($"The bill item with ID '{id}' was not found.") { }
}
