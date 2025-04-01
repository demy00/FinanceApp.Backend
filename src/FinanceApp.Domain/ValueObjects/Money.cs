namespace FinanceApp.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative.");

        Amount = amount;
        Currency = string.IsNullOrEmpty(currency) ? string.Empty : currency.ToUpper();
    }

    public override string ToString() => $"{Amount} {Currency}";
}
