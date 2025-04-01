namespace FinanceApp.Domain.Exceptions;

public class PeriodNotFoundException : FinanceAppDomainException
{
    public PeriodNotFoundException(Guid id)
        : base($"The period with ID '{id}' was not found.") { }
}
