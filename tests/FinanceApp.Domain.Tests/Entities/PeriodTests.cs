using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Tests.Entities;

public class PeriodTests
{
    private static readonly Guid userId;
    static PeriodTests() => userId = Guid.NewGuid();

    [Fact]
    public void Constructor_ShouldCreatePeriod_WithValidInputs()
    {
        var period = new Period("Monthly Expenses", "Expenses for January", new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), userId);

        Assert.Equal("Monthly Expenses", period.Name);
        Assert.Equal("Expenses for January", period.Description);
        Assert.Equal(new DateTime(2024, 1, 1), period.StartDate);
        Assert.Equal(new DateTime(2024, 1, 31), period.EndDate);
        Assert.Empty(period.Bills);
        Assert.Empty(period.TotalSpent);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenStartDateIsAfterEndDate()
    {
        Assert.Throws<ArgumentException>(() =>
            new Period("Invalid Period", "StartDate > EndDate", new DateTime(2024, 10, 10), new DateTime(2024, 10, 5), userId)
        );
    }

    [Fact]
    public void AddBill_ShouldAddBill_WhenValid()
    {
        var period = new Period("Grocery Expenses", "Weekly groceries", new DateTime(2024, 2, 2), new DateTime(2024, 2, 28), userId);
        var bill = new Bill("Groceries", "Supermarket", userId);

        period.AddBill(bill);

        Assert.Contains(bill, period.Bills);
    }

    [Fact]
    public void AddBill_ShouldThrowException_WhenBillIsNull()
    {
        var period = new Period("Shopping", "Retail expenses", new DateTime(2024, 3, 1), new DateTime(2024, 3, 31), userId);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => period.AddBill(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void AddBill_ShouldThrowException_WhenBillAlreadyExists()
    {
        var userId = Guid.NewGuid();
        var period = new Period("Monthly Bills", "All recurring bills", new DateTime(2024, 3, 1), new DateTime(2024, 3, 31), userId);
        var bill = new Bill("Electricity", "March bill", userId);

        period.AddBill(bill);

        Assert.Throws<InvalidOperationException>(() => period.AddBill(bill));
    }

    [Fact]
    public void RemoveBill_ShouldRemoveBill_WhenBillExists()
    {
        var userId = Guid.NewGuid();
        var period = new Period("Household", "Home expenses", new DateTime(2024, 4, 1), new DateTime(2024, 4, 30), userId);
        var bill = new Bill("Water Bill", "April payment", userId);

        period.AddBill(bill);
        period.RemoveBill(bill);

        Assert.DoesNotContain(bill, period.Bills);
    }

    [Fact]
    public void RemoveBill_ShouldThrowException_WhenBillIsNull()
    {
        var period = new Period(
            "Entertainment",
            "Fun expenses",
            new DateTime(2024, 5, 1),
            new DateTime(2024, 5, 31),
            userId);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => period.RemoveBill(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void RemoveBill_ShouldThrowException_WhenBillDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var period = new Period("Health", "Medical expenses", new DateTime(2024, 6, 1), new DateTime(2024, 6, 30), userId);
        var bill = new Bill("Doctor Visit", "Annual check-up", userId);

        Assert.Throws<InvalidOperationException>(() => period.RemoveBill(bill));
    }

    [Fact]
    public void TotalSpent_ShouldBeEmpty_WhenNoBillsExist()
    {
        var period = new Period("Miscellaneous", "Various expenses", new DateTime(2024, 7, 1), new DateTime(2024, 7, 31), userId);

        Assert.Empty(period.TotalSpent);
    }

    [Fact]
    public void TotalSpent_ShouldCalculateCorrectly_WhenSingleCurrencyBillsArePresent()
    {
        var userId = Guid.NewGuid();
        var period = new Period("July Expenses", "Summer expenses", new DateTime(2024, 7, 1), new DateTime(2024, 7, 31), userId);

        var bill1 = new Bill("Groceries", "Weekly shopping", userId);
        bill1.AddItem(new BillItem("Milk", "1L", DefaultCategories.Groceries, new Money(3, "EUR"), new Quantity(2), userId));
        bill1.AddItem(new BillItem("Bread", "Whole grain", DefaultCategories.Groceries, new Money(2, "EUR"), new Quantity(1), userId));

        period.AddBill(bill1);

        Assert.Single(period.TotalSpent);
        Assert.Equal(new Money(8, "EUR"), period.TotalSpent["EUR"]);
    }

    [Fact]
    public void TotalSpent_ShouldGroupByCurrency_WhenMultipleCurrenciesExist()
    {
        var userId = Guid.NewGuid();
        var period = new Period("August Expenses", "Various purchases", new DateTime(2024, 8, 1), new DateTime(2024, 8, 31), userId);

        var bill1 = new Bill("Supermarket", "Groceries", userId);
        bill1.AddItem(new BillItem("Milk", "1L", DefaultCategories.Groceries, new Money(3, "USD"), new Quantity(2), userId));

        var bill2 = new Bill("Hotel", "Business trip", userId);
        bill2.AddItem(new BillItem("Hotel Stay", "3 Nights", DefaultCategories.Groceries, new Money(300, "EUR"), new Quantity(1), userId));

        period.AddBill(bill1);
        period.AddBill(bill2);

        Assert.Equal(2, period.TotalSpent.Count);
        Assert.Equal(new Money(6, "USD"), period.TotalSpent["USD"]);
        Assert.Equal(new Money(300, "EUR"), period.TotalSpent["EUR"]);
    }

    [Fact]
    public void UpdateName_ShouldUpdatePeriodName()
    {
        var period = new Period("Old Name", "Some description", new DateTime(2024, 9, 1), new DateTime(2024, 9, 30), userId);

        period.UpdateName("New Name");

        Assert.Equal("New Name", period.Name);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdatePeriodDescription()
    {
        var period = new Period("Some Period", "Old Description", new DateTime(2024, 10, 1), new DateTime(2024, 10, 31), userId);

        period.UpdateDescription("New Description");

        Assert.Equal("New Description", period.Description);
    }

    [Fact]
    public void UpdateStartDate_ShouldUpdate_WhenValid()
    {
        var period = new Period("Test Period", "Test Description", new DateTime(2024, 11, 1), new DateTime(2024, 11, 30), userId);

        period.UpdateStartDate(new DateTime(2024, 11, 5));

        Assert.Equal(new DateTime(2024, 11, 5), period.StartDate);
    }

    [Fact]
    public void UpdateStartDate_ShouldThrowException_WhenNewStartDateIsAfterEndDate()
    {
        var period = new Period("Test Period", "Test Description", new DateTime(2024, 12, 1), new DateTime(2024, 12, 31), userId);

        Assert.Throws<ArgumentException>(() => period.UpdateStartDate(new DateTime(2025, 1, 1)));
    }

    [Fact]
    public void UpdateEndDate_ShouldThrowException_WhenNewEndDateIsBeforeStartDate()
    {
        var period = new Period("Test Period", "Test Description", new DateTime(2025, 1, 1), new DateTime(2025, 1, 31), userId);

        Assert.Throws<ArgumentException>(() => period.UpdateEndDate(new DateTime(2024, 12, 15)));
    }
}
