namespace FinanceApp.Domain.Exceptions;

public class BillNotFoundException : FinanceAppDomainException
{
    public BillNotFoundException(Guid id)
        : base($"The bill with ID '{id}' was not found.") { }
}
