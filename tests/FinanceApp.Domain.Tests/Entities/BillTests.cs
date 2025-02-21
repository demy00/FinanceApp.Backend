using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Tests.Entities;

public class BillTests
{
    [Fact]
    public void Constructor_ShouldCreateBill_WithValidInputs()
    {
        var id = Guid.NewGuid();
        var bill = new Bill(id, "Grocery", "Weekly groceries");

        Assert.Equal(id, bill.Id);
        Assert.Equal("Grocery", bill.Name);
        Assert.Equal("Weekly groceries", bill.Description);
        Assert.Empty(bill.Items);
        Assert.Equal(new Money(0, ""), bill.TotalPrice);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("House Rent", "House Rent")]
    public void Constructor_ShouldSetDefaultName_WhenNullOrEmpty(string inputName, string expectedName)
    {
        var bill = new Bill(Guid.NewGuid(), inputName, "Monthly");

        Assert.Equal(expectedName, bill.Name);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Weekly groceries", "Weekly groceries")]
    public void Constructor_ShouldSetDefaultDescription_WhenNullOrEmpty(string inputDesc, string expectedDesc)
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", inputDesc);

        Assert.Equal(expectedDesc, bill.Description);
    }

    [Fact]
    public void AddItem_ShouldAddItem_WhenValid()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");
        var item = new BillItem(Guid.NewGuid(), "Milk", "1L", new Money(2.5m, "USD"), new Quantity(2));

        bill.AddItem(item);

        Assert.Contains(item, bill.Items);
        Assert.Equal(new Money(5.0m, "USD"), bill.TotalPrice);
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenItemIsNull()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");

        Assert.Throws<ArgumentNullException>(() => bill.AddItem(null));
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenItemAlreadyExists()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");
        var item = new BillItem(Guid.NewGuid(), "Milk", "1L", new Money(2.5m, "USD"), new Quantity(2));

        bill.AddItem(item);

        Assert.Throws<InvalidOperationException>(() => bill.AddItem(item));
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItem_WhenItemExists()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");
        var item = new BillItem(Guid.NewGuid(), "Milk", "1L", new Money(2.5m, "USD"), new Quantity(2));

        bill.AddItem(item);
        bill.RemoveItem(item);

        Assert.DoesNotContain(item, bill.Items);
        Assert.Equal(new Money(0, ""), bill.TotalPrice);
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenNewItemHasDifferentCurrency()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");
        var item1 = new BillItem(Guid.NewGuid(), "Milk", "1L", new Money(2.5m, "USD"), new Quantity(2));
        var item2 = new BillItem(Guid.NewGuid(), "Milk", "1L", new Money(2.5m, "EUR"), new Quantity(2));

        bill.AddItem(item1);

        Assert.Throws<InvalidOperationException>(() => bill.AddItem(item2));
    }

    [Fact]
    public void RemoveItem_ShouldThrowException_WhenItemIsNull()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");

        Assert.Throws<ArgumentNullException>(() => bill.RemoveItem(null));
    }

    [Fact]
    public void RemoveItem_ShouldThrowException_WhenItemDoesNotExist()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");
        var item = new BillItem(Guid.NewGuid(), "Milk", "1L", new Money(2.5m, "USD"), new Quantity(2));

        Assert.Throws<InvalidOperationException>(() => bill.RemoveItem(item));
    }

    [Fact]
    public void UpdateName_ShouldUpdateName_WhenValid()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");

        bill.UpdateName("Updated Bill Name");

        Assert.Equal("Updated Bill Name", bill.Name);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription_WhenValid()
    {
        var bill = new Bill(Guid.NewGuid(), "Grocery", "Weekly groceries");

        bill.UpdateDescription("Updated Description");

        Assert.Equal("Updated Description", bill.Description);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Electricity Bill", "Electricity Bill")]
    public void UpdateName_ShouldHandleNullOrEmptyInput(string inputName, string expectedName)
    {
        var bill = new Bill(Guid.NewGuid(), "Initial Name", "Description");

        bill.UpdateName(inputName);

        Assert.Equal(expectedName, bill.Name);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Rent Payment", "Rent Payment")]
    public void UpdateDescription_ShouldHandleNullOrEmptyInput(string inputDesc, string expectedDesc)
    {
        var bill = new Bill(Guid.NewGuid(), "Bill Name", "Initial Description");

        bill.UpdateDescription(inputDesc);

        Assert.Equal(expectedDesc, bill.Description);
    }

    [Theory]
    [InlineData(10, 2, 20)]
    [InlineData(15, 3, 45)]
    public void TotalPrice_ShouldCalculateCorrectly(decimal price, int quantity, decimal expectedTotal)
    {
        var bill = new Bill(Guid.NewGuid(), "Invoice", "Monthly invoice");
        var item = new BillItem(Guid.NewGuid(), "Product", "Item Description", new Money(price, "USD"), new Quantity(quantity));

        bill.AddItem(item);

        Assert.Equal(new Money(expectedTotal, "USD"), bill.TotalPrice);
    }

    [Fact]
    public void TotalPrice_ShouldReturnZero_WhenNoItems()
    {
        var bill = new Bill(Guid.NewGuid(), "Empty Bill", "No items");

        Assert.Equal(new Money(0, ""), bill.TotalPrice);
    }

    [Fact]
    public void Bill_ShouldBeEqual_WhenIdIsSame()
    {
        var id = Guid.NewGuid();
        var bill1 = new Bill(id, "House Rent", "January Rent");
        var bill2 = new Bill(id, "House Rent", "January Rent");

        Assert.Equal(bill1, bill2);
    }

    [Fact]
    public void Bill_ShouldNotBeEqual_WhenIdIsDifferent()
    {
        var bill1 = new Bill(Guid.NewGuid(), "House Rent", "January Rent");
        var bill2 = new Bill(Guid.NewGuid(), "House Rent", "January Rent");

        Assert.NotEqual(bill1, bill2);
    }
}
