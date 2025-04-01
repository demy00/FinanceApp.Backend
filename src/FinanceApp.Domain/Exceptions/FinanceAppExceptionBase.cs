using System.Runtime.Serialization;

namespace FinanceApp.Domain.Exceptions;

public abstract class FinanceAppDomainException : Exception
{
    protected FinanceAppDomainException()
    {
    }

    protected FinanceAppDomainException(string? message) : base(message)
    {
    }

    protected FinanceAppDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    protected FinanceAppDomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
