namespace FinanceApp.Domain.ValueObjects;

public record Quantity
{
    public int Value { get; init; }

    public Quantity(int value)
    {
        if (value <= 0) throw new ArgumentException("Value must be greater than zero.");
        Value = value;
    }
}
