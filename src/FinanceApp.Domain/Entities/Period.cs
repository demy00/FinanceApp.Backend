using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Entities;

public class Period : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public IReadOnlyDictionary<string, Money> TotalSpent => GetTotalSpent();

    private readonly List<Bill> _bills = new();
    public IReadOnlyCollection<Bill> Bills => _bills.AsReadOnly();
    public Guid UserId { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Period() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Period(string name, string description, DateTime startDate, DateTime endDate, Guid userId)
    {
        if (startDate > endDate) throw new ArgumentException("StartDate must be before or equal to EndDate.");

        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        Description = description ?? string.Empty;
        StartDate = startDate;
        EndDate = endDate;
        UserId = userId;
    }

    public void AddBill(Bill bill)
    {
        if (bill is null) throw new ArgumentNullException(nameof(bill));
        if (_bills.Contains(bill)) throw new InvalidOperationException("Bill already exists in the period.");

        _bills.Add(bill);
    }

    public void RemoveBill(Bill bill)
    {
        if (bill is null) throw new ArgumentNullException(nameof(bill));
        if (!_bills.Remove(bill)) throw new InvalidOperationException("Bill not found in the period.");
    }

    public void UpdateName(string name)
    {
        Name = name ?? string.Empty;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
    }

    private Dictionary<string, Money> GetTotalSpent()
    {
        if (_bills.Count == 0) return new Dictionary<string, Money>();

        return _bills
            .GroupBy(b => b.TotalPrice.Currency)
            .ToDictionary(
                g => g.Key,
                g => new Money(g.Sum(b => b.TotalPrice.Amount), g.Key)
            );
    }

    public void UpdateStartDate(DateTime newStartDate)
    {
        if (newStartDate > EndDate) throw new ArgumentException("StartDate cannot be later than EndDate.");
        StartDate = newStartDate;
    }

    public void UpdateEndDate(DateTime newEndDate)
    {
        if (newEndDate < StartDate) throw new ArgumentException("EndDate cannot be earlier than StartDate.");
        EndDate = newEndDate;
    }
}
