using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Infrastructure.Tests;

public static class TestDataHelper
{
    // Create a 4 sample categories.
    public static List<Category> GetSampleCategories(Guid userId)
    {
        return new List<Category>
            {
                new Category("Food", "Expenses related to food", userId),
                new Category("Transport", "Expenses for transportation", userId),
                new Category("Entertainment", "Expenses for leisure", userId),
                new Category("Utilities", "Expenses for utilities", userId)
            };
    }

    // Create 8 BillItems that can be reused in Bills.
    public static List<BillItem> GetSampleBillItems(Guid userId, List<Category> categories)
    {
        return new List<BillItem>
            {
                new BillItem("0", "0", categories[0], new Money(3m, "USD"), new Quantity(1), userId),
                new BillItem("1", "1", categories[0], new Money(15m, "USD"), new Quantity(1), userId),
                new BillItem("2", "2", categories[1], new Money(10m, "USD"), new Quantity(2), userId),
                new BillItem("3", "3", categories[1], new Money(50m, "USD"), new Quantity(1), userId),
                new BillItem("4", "4", categories[2], new Money(12m, "USD"), new Quantity(1), userId),

                new BillItem("5", "5", categories[2], new Money(3m, "EUR"), new Quantity(1), userId),
                new BillItem("6", "6", categories[2], new Money(4m, "EUR"), new Quantity(1), userId),
                new BillItem("7", "7", categories[3], new Money(2.5m, "EUR"), new Quantity(1), userId),
                new BillItem("8", "8", categories[3], new Money(2.5m, "EUR"), new Quantity(1), userId)
            };
    }

    // Create 10 Bills with varied BillItems.
    public static List<Bill> GetSampleBills(Guid userId, List<BillItem> billItems)
    {
        var bills = new List<Bill>();

        for (var i = 1; i <= 10; i++)
        {
            var bill = new Bill($"Bill {i}", $"Description for Bill {i}", userId);

            if (i % 2 == 0)
            {
                bill.AddBillItem(CloneBillItem(billItems[0]));
                bill.AddBillItem(CloneBillItem(billItems[1]));
            }
            else
            {
                bill.AddBillItem(CloneBillItem(billItems[5]));
                bill.AddBillItem(CloneBillItem(billItems[6]));
            }

            bills.Add(bill);
        }
        return bills;
    }

    // Create 5 Periods, each containing 3 bills.
    public static List<Period> GetSamplePeriods(Guid userId)
    {
        var periods = new List<Period>();

        for (var i = 1; i <= 5; i++)
        {
            var period = new Period(
                name: $"Period {i}",
                description: $"Expenses for period {i}",
                startDate: new DateTime(2024, i, 1),
                endDate: new DateTime(2024, i, 28),
                userId: userId);

            var bills = GetSampleBills(userId, GetSampleBillItems(userId, GetSampleCategories(userId))).Take(3).ToList();

            foreach (var bill in bills)
            {
                period.AddBill(bill);
            }
            periods.Add(period);
        }
        return periods;
    }

    private static BillItem CloneBillItem(BillItem item)
    {
        return new BillItem(
            name: item.Name,
            description: item.Description,
            category: item.Category,
            price: new Money(item.Price.Amount, item.Price.Currency),
            quantity: new Quantity(item.Quantity.Value),
            userId: item.UserId);
    }

    //// Helper method to clone a BillItem so each Bill gets its own instance.
    //private BillItem CloneBillItem(BillItem item)
    //{
    //    return new BillItem(
    //        name: item.Name,
    //        description: item.Description,
    //        category: item.Category,
    //        price: new Money(item.Price.Amount, item.Price.Currency),
    //        quantity: new Quantity(item.Quantity.Value),
    //        userId: item.UserId);
    //}

    //// Helper method to clone a Bill including its BillItems.
    //private Bill CloneBill(Bill bill)
    //{
    //    var newBill = new Bill(bill.Name, bill.Description, bill.UserId);
    //    foreach (var bi in bill.BillItems)
    //    {
    //        newBill.AddBillItem(CloneBillItem(bi));
    //    }
    //    return newBill;
    //}

    //// Helper method to clone a Period including its Bills (and their BillItems).
    //private Period ClonePeriod(Period period)
    //{
    //    var newPeriod = new Period(period.Name, period.Description, period.StartDate, period.EndDate, period.UserId);
    //    foreach (var bill in period.Bills)
    //    {
    //        newPeriod.AddBill(CloneBill(bill));
    //    }
    //    return newPeriod;
    //}

}
