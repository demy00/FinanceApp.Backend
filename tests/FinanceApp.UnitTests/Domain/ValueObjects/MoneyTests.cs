using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenAmountIsNegative()
    {
        Assert.Throws<ArgumentException>(() => new Money(-1, "USD"));
    }

    [Theory]
#pragma warning disable xUnit1012 // Null should only be used for nullable parameters
    [InlineData(null, "")]
#pragma warning restore xUnit1012 // Null should only be used for nullable parameters
    [InlineData("", "")]
    [InlineData(" ", " ")]
    public void Constructor_ShouldDefaultToEmptyString_WhenCurrencyIsNullOrWhiteSpace(string currency, string expectedCurrency)
    {
        var money = new Money(100, currency);

        Assert.Equal(expectedCurrency, money.Currency);
    }

    [Fact]
    public void Constructor_ShouldCreateMoney_WhenValidValuesAreGiven()
    {
        var money = new Money(100.50m, "usd");

        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Money_ShouldBeEqual_WhenAmountAndCurrencyAreSame()
    {
        var money1 = new Money(50, "EUR");
        var money2 = new Money(50, "EUR");

        Assert.Equal(money1, money2);
    }

    [Fact]
    public void Money_ShouldNotBeEqual_WhenAmountOrCurrencyAreDifferent()
    {
        var money1 = new Money(50, "EUR");
        var money2 = new Money(60, "EUR");
        var money3 = new Money(50, "USD");

        Assert.NotEqual(money1, money2);
        Assert.NotEqual(money1, money3);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var money = new Money(99.99m, "usd");

        var result = money.ToString();

        Assert.Equal("99.99 USD", result);
    }
}