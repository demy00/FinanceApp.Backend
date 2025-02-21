using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Tests.Entities;

public class BillItemTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        var billItem = new BillItem(Guid.NewGuid(), null, null, null, null);

        Assert.Equal("", billItem.Name);
        Assert.Equal("", billItem.Description);
        Assert.Equal(new Money(0, ""), billItem.Price);
        Assert.Equal(new Quantity(1), billItem.Quantity);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("Name", "Name")]
    public void Constructor_ShouldSetName(string inputName, string expectedName)
    {
        var billItem = new BillItem(Guid.NewGuid(), inputName, "Description", new Money(100, "USD"), new Quantity(1));
        Assert.Equal(expectedName, billItem.Name);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("Description", "Description")]
    public void Constructor_ShouldSetDescription(string inputDescription, string expectedDescription)
    {
        var billItem = new BillItem(Guid.NewGuid(), "", inputDescription, new Money(100, "USD"), new Quantity(1));
        Assert.Equal(expectedDescription, billItem.Description);
    }

    [Fact]
    public void Constructor_ShouldSetPrice()
    {
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(500, "USD"), new Quantity(1));
        Assert.Equal(new Money(500, "USD"), billItem.Price);
    }

    [Fact]
    public void Constructor_ShouldSetQuantity()
    {
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(100, "USD"), new Quantity(5));
        Assert.Equal(new Quantity(5), billItem.Quantity);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenPriceIsNegative()
    {
        Assert.Throws<ArgumentException>(() =>
            new BillItem(Guid.NewGuid(), "Name", "Description", new Money(-100, "USD"), new Quantity(5)));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenQuantityIsNegative()
    {
        Assert.Throws<ArgumentException>(() =>
            new BillItem(Guid.NewGuid(), "Name", "Description", new Money(100, "USD"), new Quantity(-5)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("New")]
    public void UpdateName_ShouldUpdateName(string name)
    {
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(10, "USD"), new Quantity(1));

        billItem.UpdateName(name);

        Assert.Equal(name, billItem.Name);
    }

    [Fact]
    public void UpdateName_ShouldUpdateName_WhenNameIsNull()
    {
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(10, "USD"), new Quantity(1));

        billItem.UpdateName(null);

        Assert.Equal("", billItem.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("New")]
    public void UpdateDescription_ShouldUpdateDescription(string description)
    {
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(10, "USD"), new Quantity(1));

        billItem.UpdateDescription(description);

        Assert.Equal(description, billItem.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription_WhenDescriptionIsNull()
    {
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(10, "USD"), new Quantity(1));

        billItem.UpdateDescription(null);

        Assert.Equal("", billItem.Description);
    }

    [Fact]
    public void UpdatePrice_ShouldThrowException_WhenPriceIsNull()
    {
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(10, "USD"), new Quantity(1));

        Assert.Throws<ArgumentNullException>(() => billItem.UpdatePrice(null));
    }

    [Fact]
    public void UpdateQuantity_ShouldThrowException_WhenQuantityIsNull()
    {
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(10, "USD"), new Quantity(1));

        Assert.Throws<ArgumentNullException>(() => billItem.UpdateQuantity(null));
    }

    [Fact]
    public void TotalPrice_ShouldCalculateCorrectly()
    {
        var price = new Money(15.00m, "USD");
        var quantity = new Quantity(3);
        var billItem = new BillItem(Guid.NewGuid(), "Name", "Description", price, quantity);

        var expectedTotal = new Money(15.00m * 3, "USD");

        Assert.Equal(expectedTotal, billItem.TotalPrice);
    }

    [Fact]
    public void BillItem_ShouldBeEqual_WhenIdIsSame()
    {
        var id = Guid.NewGuid();
        var item1 = new BillItem(id, "Name", "Description", new Money(1, "USD"), new Quantity(1));
        var item2 = new BillItem(id, "Name", "Description", new Money(1, "USD"), new Quantity(1));

        Assert.Equal(item1, item2);
    }

    [Fact]
    public void BillItem_ShouldNotBeEqual_WhenIdIsDifferent()
    {
        var item1 = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(1, "USD"), new Quantity(1));
        var item2 = new BillItem(Guid.NewGuid(), "Name", "Description", new Money(1, "USD"), new Quantity(1));

        Assert.NotEqual(item1, item2);
    }
}
