using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Tests.Entities;

public class PeriodTests
{
    [Fact]
    public void Constructor_ShouldCreatePeriod_WithValidInputs()
    {
        var id = Guid.NewGuid();
        var period = new Period(id, "Monthly Expenses", "Expenses for January", new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.Equal(id, period.Id);
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
            new Period(Guid.NewGuid(), "Invalid Period", "StartDate > EndDate", new DateTime(2024, 1, 10), new DateTime(2024, 1, 5))
        );
    }

    // ✅ 2. Bill Management (Add & Remove) ✅

    [Fact]
    public void AddBill_ShouldAddBill_WhenValid()
    {
        var period = new Period(Guid.NewGuid(), "Grocery Expenses", "Weekly groceries", new DateTime(2024, 2, 1), new DateTime(2024, 2, 28));
        var bill = new Bill(Guid.NewGuid(), "Groceries", "Supermarket");

        period.AddBill(bill);

        Assert.Contains(bill, period.Bills);
    }

    [Fact]
    public void AddBill_ShouldThrowException_WhenBillIsNull()
    {
        var period = new Period(Guid.NewGuid(), "Shopping", "Retail expenses", new DateTime(2024, 3, 1), new DateTime(2024, 3, 31));

        Assert.Throws<ArgumentNullException>(() => period.AddBill(null));
    }

    [Fact]
    public void AddBill_ShouldThrowException_WhenBillAlreadyExists()
    {
        var period = new Period(Guid.NewGuid(), "Monthly Bills", "All recurring bills", new DateTime(2024, 3, 1), new DateTime(2024, 3, 31));
        var bill = new Bill(Guid.NewGuid(), "Electricity", "March bill");

        period.AddBill(bill);

        Assert.Throws<InvalidOperationException>(() => period.AddBill(bill));
    }

    [Fact]
    public void RemoveBill_ShouldRemoveBill_WhenBillExists()
    {
        var period = new Period(Guid.NewGuid(), "Household", "Home expenses", new DateTime(2024, 4, 1), new DateTime(2024, 4, 30));
        var bill = new Bill(Guid.NewGuid(), "Water Bill", "April payment");

        period.AddBill(bill);
        period.RemoveBill(bill);

        Assert.DoesNotContain(bill, period.Bills);
    }

    [Fact]
    public void RemoveBill_ShouldThrowException_WhenBillIsNull()
    {
        var period = new Period(Guid.NewGuid(), "Entertainment", "Fun expenses", new DateTime(2024, 5, 1), new DateTime(2024, 5, 31));

        Assert.Throws<ArgumentNullException>(() => period.RemoveBill(null));
    }

    [Fact]
    public void RemoveBill_ShouldThrowException_WhenBillDoesNotExist()
    {
        var period = new Period(Guid.NewGuid(), "Health", "Medical expenses", new DateTime(2024, 6, 1), new DateTime(2024, 6, 30));
        var bill = new Bill(Guid.NewGuid(), "Doctor Visit", "Annual check-up");

        Assert.Throws<InvalidOperationException>(() => period.RemoveBill(bill));
    }

    // ✅ 3. TotalSpent Calculation (Multi-Currency Support) ✅

    [Fact]
    public void TotalSpent_ShouldBeEmpty_WhenNoBillsExist()
    {
        var period = new Period(Guid.NewGuid(), "Miscellaneous", "Various expenses", new DateTime(2024, 7, 1), new DateTime(2024, 7, 31));

        Assert.Empty(period.TotalSpent);
    }

    [Fact]
    public void TotalSpent_ShouldCalculateCorrectly_WhenSingleCurrencyBillsArePresent()
    {
        var period = new Period(Guid.NewGuid(), "July Expenses", "Summer expenses", new DateTime(2024, 7, 1), new DateTime(2024, 7, 31));

        var bill1 = new Bill(Guid.NewGuid(), "Groceries", "Weekly shopping");
        bill1.AddItem(new BillItem(Guid.NewGuid(), "Milk", "1L", new Money(3, "EUR"), new Quantity(2)));
        bill1.AddItem(new BillItem(Guid.NewGuid(), "Bread", "Whole grain", new Money(2, "EUR"), new Quantity(1)));

        period.AddBill(bill1);

        Assert.Single(period.TotalSpent);
        Assert.Equal(new Money(8, "EUR"), period.TotalSpent["EUR"]);
    }

    [Fact]
    public void TotalSpent_ShouldGroupByCurrency_WhenMultipleCurrenciesExist()
    {
        var period = new Period(Guid.NewGuid(), "August Expenses", "Various purchases", new DateTime(2024, 8, 1), new DateTime(2024, 8, 31));

        var bill1 = new Bill(Guid.NewGuid(), "Supermarket", "Groceries");
        bill1.AddItem(new BillItem(Guid.NewGuid(), "Milk", "1L", new Money(3, "USD"), new Quantity(2)));

        var bill2 = new Bill(Guid.NewGuid(), "Hotel", "Business trip");
        bill2.AddItem(new BillItem(Guid.NewGuid(), "Hotel Stay", "3 Nights", new Money(300, "EUR"), new Quantity(1)));

        period.AddBill(bill1);
        period.AddBill(bill2);

        Assert.Equal(2, period.TotalSpent.Count);
        Assert.Equal(new Money(6, "USD"), period.TotalSpent["USD"]);
        Assert.Equal(new Money(300, "EUR"), period.TotalSpent["EUR"]);
    }

    // ✅ 4. Update Name, Description, and Dates ✅

    [Fact]
    public void UpdateName_ShouldUpdatePeriodName()
    {
        var period = new Period(Guid.NewGuid(), "Old Name", "Some description", new DateTime(2024, 9, 1), new DateTime(2024, 9, 30));

        period.UpdateName("New Name");

        Assert.Equal("New Name", period.Name);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdatePeriodDescription()
    {
        var period = new Period(Guid.NewGuid(), "Some Period", "Old Description", new DateTime(2024, 10, 1), new DateTime(2024, 10, 31));

        period.UpdateDescription("New Description");

        Assert.Equal("New Description", period.Description);
    }

    [Fact]
    public void UpdateStartDate_ShouldUpdate_WhenValid()
    {
        var period = new Period(Guid.NewGuid(), "Test Period", "Test Description", new DateTime(2024, 11, 1), new DateTime(2024, 11, 30));

        period.UpdateStartDate(new DateTime(2024, 11, 5));

        Assert.Equal(new DateTime(2024, 11, 5), period.StartDate);
    }

    [Fact]
    public void UpdateStartDate_ShouldThrowException_WhenNewStartDateIsAfterEndDate()
    {
        var period = new Period(Guid.NewGuid(), "Test Period", "Test Description", new DateTime(2024, 12, 1), new DateTime(2024, 12, 31));

        Assert.Throws<ArgumentException>(() => period.UpdateStartDate(new DateTime(2025, 1, 1)));
    }

    [Fact]
    public void UpdateEndDate_ShouldThrowException_WhenNewEndDateIsBeforeStartDate()
    {
        var period = new Period(Guid.NewGuid(), "Test Period", "Test Description", new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));

        Assert.Throws<ArgumentException>(() => period.UpdateEndDate(new DateTime(2024, 12, 15)));
    }
}
