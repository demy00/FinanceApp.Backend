using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Tests;

public class QuantityTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenValueIsZero()
    {
        Assert.Throws<ArgumentException>(() => new Quantity(0));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenValueIsNegative()
    {
        Assert.Throws<ArgumentException>(() => new Quantity(-5));
    }

    [Fact]
    public void Constructor_ShouldCreateQuantity_WhenValidValueIsGiven()
    {
        var quantity = new Quantity(10);

        Assert.Equal(10, quantity.Value);
    }

    [Fact]
    public void Quantity_ShouldBeEqual_WhenValuesAreSame()
    {
        var quantity1 = new Quantity(5);
        var quantity2 = new Quantity(5);

        Assert.Equal(quantity1, quantity2);
    }

    [Fact]
    public void Quantity_ShouldNotBeEqual_WhenValuesAreDifferent()
    {
        var quantity1 = new Quantity(5);
        var quantity2 = new Quantity(10);

        Assert.NotEqual(quantity1, quantity2);
    }
}
