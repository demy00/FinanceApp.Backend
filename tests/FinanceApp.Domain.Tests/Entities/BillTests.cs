using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Tests.Entities;

public class BillTests
{
    private static readonly Guid userId;
    static BillTests() => userId = Guid.NewGuid();

    [Fact]
    public void Constructor_ShouldCreateBill_WithValidInputs()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);

        Assert.Equal("Grocery", bill.Name);
        Assert.Equal("Weekly groceries", bill.Description);
        Assert.Empty(bill.Items);
        Assert.Equal(new Money(0, ""), bill.TotalPrice);
        Assert.Equal(userId, bill.UserId);
    }

    [Theory]
#pragma warning disable xUnit1012 // Null should only be used for nullable parameters
    [InlineData(null, "")]
#pragma warning restore xUnit1012 // Null should only be used for nullable parameters
    [InlineData("", "")]
    [InlineData("House Rent", "House Rent")]
    public void Constructor_ShouldSetDefaultName_WhenNullOrEmpty(string inputName, string expectedName)
    {
        var bill = new Bill(inputName, "Monthly", userId);

        Assert.Equal(expectedName, bill.Name);
    }

    [Theory]
#pragma warning disable xUnit1012 // Null should only be used for nullable parameters
    [InlineData(null, "")]
#pragma warning restore xUnit1012 // Null should only be used for nullable parameters
    [InlineData("", "")]
    [InlineData("Weekly groceries", "Weekly groceries")]
    public void Constructor_ShouldSetDefaultDescription_WhenNullOrEmpty(string inputDesc, string expectedDesc)
    {
        var bill = new Bill("Grocery", inputDesc, userId);

        Assert.Equal(expectedDesc, bill.Description);
    }

    [Fact]
    public void AddItem_ShouldAddItem_WhenValid()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);
        var item = new BillItem("Milk", "1L", DefaultCategories.Groceries, new Money(2.5m, "USD"), new Quantity(2), userId);

        bill.AddItem(item);

        Assert.Contains(item, bill.Items);
        Assert.Equal(new Money(5.0m, "USD"), bill.TotalPrice);
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenItemIsNull()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => bill.AddItem(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenItemAlreadyExists()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);
        var item = new BillItem("Milk", "1L", DefaultCategories.Groceries, new Money(2.5m, "USD"), new Quantity(2), userId);

        bill.AddItem(item);

        Assert.Throws<InvalidOperationException>(() => bill.AddItem(item));
    }

    [Fact]
    public void AddMultipleItems_ShouldCalculateAccurateTotalPrice()
    {
        var bill = new Bill("Multiple Items Bill", "Test for multiple items", userId);
        var item1 = new BillItem("Item1", "First item", DefaultCategories.Groceries, new Money(3m, "USD"), new Quantity(2), userId);
        var item2 = new BillItem("Item2", "Second item", DefaultCategories.Groceries, new Money(4m, "USD"), new Quantity(3), userId);

        bill.AddItem(item1);
        bill.AddItem(item2);

        Assert.Equal(new Money(18m, "USD"), bill.TotalPrice);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItem_WhenItemExists()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);
        var item = new BillItem("Milk", "1L", DefaultCategories.Groceries, new Money(2.5m, "USD"), new Quantity(2), userId);

        bill.AddItem(item);
        bill.RemoveItem(item);

        Assert.DoesNotContain(item, bill.Items);
        Assert.Equal(new Money(0, ""), bill.TotalPrice);
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenNewItemHasDifferentCurrency()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);
        var item1 = new BillItem("Milk", "1L", DefaultCategories.Groceries, new Money(2.5m, "USD"), new Quantity(2), userId);
        var item2 = new BillItem("Milk", "1L", DefaultCategories.Groceries, new Money(2.5m, "EUR"), new Quantity(2), userId);

        bill.AddItem(item1);

        Assert.Throws<InvalidOperationException>(() => bill.AddItem(item2));
    }

    [Fact]
    public void RemoveItem_ShouldThrowException_WhenItemIsNull()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => bill.RemoveItem(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void RemoveItem_ShouldThrowException_WhenItemDoesNotExist()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);
        var item = new BillItem("Milk", "1L", DefaultCategories.Groceries, new Money(2.5m, "USD"), new Quantity(2), userId);

        Assert.Throws<InvalidOperationException>(() => bill.RemoveItem(item));
    }

    [Fact]
    public void RemoveItem_ShouldUpdateTotalPriceCorrectly()
    {
        var bill = new Bill("Update Total Bill", "Test removal effect", userId);
        var item1 = new BillItem("Item1", "First item", DefaultCategories.Groceries, new Money(5m, "USD"), new Quantity(2), userId);
        var item2 = new BillItem("Item2", "Second item", DefaultCategories.Groceries, new Money(7m, "USD"), new Quantity(1), userId);

        bill.AddItem(item1);
        bill.AddItem(item2);

        Assert.Equal(new Money(17m, "USD"), bill.TotalPrice);

        bill.RemoveItem(item1);
        Assert.Equal(new Money(7m, "USD"), bill.TotalPrice);
    }

    [Fact]
    public void AddItem_AfterRemovingAllItems_AllowsDifferentCurrency()
    {
        var bill = new Bill("Currency Change Bill", "Test different currency after removal", userId);
        var item1 = new BillItem("Item1", "First item", DefaultCategories.Groceries, new Money(5m, "USD"), new Quantity(1), userId);
        bill.AddItem(item1);
        bill.RemoveItem(item1);

        var item2 = new BillItem("Item2", "Second item", DefaultCategories.Groceries, new Money(10m, "EUR"), new Quantity(1), userId);
        Exception ex = Record.Exception(() => bill.AddItem(item2));

        Assert.Null(ex);
        Assert.Equal(new Money(10m, "EUR"), bill.TotalPrice);
    }

    [Fact]
    public void UpdateName_ShouldUpdateName_WhenValid()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);

        bill.UpdateName("Updated Bill Name");

        Assert.Equal("Updated Bill Name", bill.Name);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription_WhenValid()
    {
        var bill = new Bill("Grocery", "Weekly groceries", userId);

        bill.UpdateDescription("Updated Description");

        Assert.Equal("Updated Description", bill.Description);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Electricity Bill", "Electricity Bill")]
    public void UpdateName_ShouldHandleNullOrEmptyInput(string inputName, string expectedName)
    {
        var bill = new Bill("Initial Name", "Description", userId);

        bill.UpdateName(inputName);

        Assert.Equal(expectedName, bill.Name);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Rent Payment", "Rent Payment")]
    public void UpdateDescription_ShouldHandleNullOrEmptyInput(string inputDesc, string expectedDesc)
    {
        var bill = new Bill("Bill Name", "Initial Description", userId);

        bill.UpdateDescription(inputDesc);

        Assert.Equal(expectedDesc, bill.Description);
    }

    [Theory]
    [InlineData(10, 2, 20)]
    [InlineData(15, 3, 45)]
    public void TotalPrice_ShouldCalculateCorrectly(decimal price, int quantity, decimal expectedTotal)
    {
        var bill = new Bill("Invoice", "Monthly invoice", userId);
        var item = new BillItem("Product", "Item Description", DefaultCategories.Groceries, new Money(price, "USD"), new Quantity(quantity), userId);

        bill.AddItem(item);

        Assert.Equal(new Money(expectedTotal, "USD"), bill.TotalPrice);
    }

    [Fact]
    public void TotalPrice_ShouldReturnZero_WhenNoItems()
    {
        var bill = new Bill("Empty Bill", "No items", userId);

        Assert.Equal(new Money(0, ""), bill.TotalPrice);
    }

    [Fact]
    public void Bill_ShouldNotBeEqual_WhenIdIsDifferent()
    {
        var bill1 = new Bill("House Rent", "January Rent", userId);
        var bill2 = new Bill("House Rent", "January Rent", userId);

        Assert.NotEqual(bill1, bill2);
    }

    [Fact]
    public void UserId_ShouldRemainUnchangedAfterUpdates()
    {
        var userId = Guid.NewGuid();
        var bill = new Bill("Test Bill", "Test description", userId);

        bill.UpdateName("Updated Name");
        bill.UpdateDescription("Updated Description");

        Assert.Equal(userId, bill.UserId);
    }

    [Fact]
    public void ItemsProperty_ShouldBeReadOnly()
    {
        var bill = new Bill("ReadOnly Items Bill", "Test Items property immutability", userId);
        var item = new BillItem("Item", "Test item", DefaultCategories.Groceries, new Money(3m, "USD"), new Quantity(1), userId);
        bill.AddItem(item);

        var items = bill.Items;
        Assert.IsType<System.Collections.ObjectModel.ReadOnlyCollection<BillItem>>(items);

        Assert.Single(items);
    }
}
