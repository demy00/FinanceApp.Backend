namespace FinanceApp.Application.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
