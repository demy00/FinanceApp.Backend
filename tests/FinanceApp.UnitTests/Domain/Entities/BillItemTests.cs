using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.UnitTests.Domain.Entities;

public class BillItemTests
{
    private static readonly Guid userId;
    static BillItemTests() => userId = Guid.NewGuid();

    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var billItem = new BillItem("Name", null, null, null, null, userId);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        Assert.Equal("Name", billItem.Name);
        Assert.Equal("", billItem.Description);
        Assert.Equal(new Money(0, ""), billItem.Price);
        Assert.Equal(DefaultCategories.Other, billItem.Category);
        Assert.Equal("Other", billItem.Category!.Name);
        Assert.Equal(new Quantity(1), billItem.Quantity);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsNullOrWhiteSpace()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentException>(() => new BillItem(null, null, null, null, null, userId));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("Description", "Description")]
    public void Constructor_ShouldSetDescription(string inputDescription, string expectedDescription)
    {
        var billItem = new BillItem("Name", inputDescription, DefaultCategories.Other, new Money(100, "USD"), new Quantity(1), userId);
        Assert.Equal(expectedDescription, billItem.Description);
    }

    [Fact]
    public void Constructor_ShouldSetPrice()
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(500, "USD"), new Quantity(1), userId);
        Assert.Equal(new Money(500, "USD"), billItem.Price);
    }

    [Fact]
    public void Constructor_ShouldSetQuantity()
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(100, "USD"), new Quantity(5), userId);
        Assert.Equal(new Quantity(5), billItem.Quantity);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenPriceIsNegative()
    {
        Assert.Throws<ArgumentException>(() =>
            new BillItem("Name", "Description", DefaultCategories.Other, new Money(-100, "USD"), new Quantity(5), userId));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenQuantityIsNegative()
    {
        Assert.Throws<ArgumentException>(() =>
            new BillItem("Name", "Description", DefaultCategories.Other, new Money(100, "USD"), new Quantity(-5), userId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
#pragma warning disable xUnit1012 // Null should only be used for nullable parameters
    [InlineData(null)]
#pragma warning restore xUnit1012 // Null should only be used for nullable parameters
    public void UpdateName_ShouldThrowException_WhenNameIsNullOrWhiteSpace(string name)
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(10, "USD"), new Quantity(1), userId);

        Assert.Throws<ArgumentException>(() => billItem.UpdateName(name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("New")]
    public void UpdateDescription_ShouldUpdateDescription(string description)
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(10, "USD"), new Quantity(1), userId);

        billItem.UpdateDescription(description);

        Assert.Equal(description, billItem.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription_WhenDescriptionIsNull()
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(10, "USD"), new Quantity(1), userId);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        billItem.UpdateDescription(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        Assert.Equal("", billItem.Description);
    }

    [Fact]
    public void UpdatePrice_ShouldThrowException_WhenPriceIsNull()
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(10, "USD"), new Quantity(1), userId);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => billItem.UpdatePrice(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void UpdatePrice_ShouldUpdatePrice()
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(10, "USD"), new Quantity(1), userId);
        var newPrice = new Money(20, "USD");
        billItem.UpdatePrice(newPrice);
        Assert.Equal(newPrice, billItem.Price);
    }

    [Fact]
    public void UpdateQuantity_ShouldThrowException_WhenQuantityIsNull()
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(10, "USD"), new Quantity(1), userId);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => billItem.UpdateQuantity(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void UpdateQuantity_ShouldUpdateQuantity()
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(10, "USD"), new Quantity(1), userId);
        var newQuantity = new Quantity(3);
        billItem.UpdateQuantity(newQuantity);
        Assert.Equal(newQuantity, billItem.Quantity);
    }

    [Theory]
    [InlineData("Food")]
    [InlineData("Travel")]
    public void UpdateCategory_ShouldUpdateCategory(string newCategoryName)
    {
        var billItem = new BillItem("Name", "Description", DefaultCategories.Other, new Money(10, "USD"), new Quantity(1), userId);
        var newCategory = new Category(newCategoryName, "", userId);
        billItem.UpdateCategory(newCategory);
        Assert.Equal(newCategory, billItem.Category);
    }

    [Fact]
    public void UpdateCategory_ShouldDefaultToOther_WhenNull()
    {
        var billItem = new BillItem("Name", "Description", new Category("Custom", "", userId), new Money(10, "USD"), new Quantity(1), userId);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        billItem.UpdateCategory(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Equal(DefaultCategories.Other, billItem.Category);
    }

    [Fact]
    public void BillItem_ShouldNotBeEqual_WhenIdIsDifferent()
    {
        var item1 = new BillItem("Name", "Description", DefaultCategories.Other, new Money(1, "USD"), new Quantity(1), userId);
        var item2 = new BillItem("Name", "Description", DefaultCategories.Other, new Money(1, "USD"), new Quantity(1), userId);

        Assert.NotEqual(item1, item2);
    }
}
