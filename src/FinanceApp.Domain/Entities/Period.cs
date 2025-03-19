using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Entities;

public class Period : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public IReadOnlyCollection<Money> TotalSpent => CalculateTotalSpent();

    private readonly List<Bill> _bills = new();
    public IReadOnlyCollection<Bill> Bills => _bills.AsReadOnly();
    public Guid UserId { get; init; }

#pragma warning disable CS8618  // Required for EF Core.
    private Period() { }
#pragma warning restore CS8618

    public Period(string name, string description, DateTime startDate, DateTime endDate, Guid userId)
        : base()
    {
        ValidateDates(startDate, endDate);
        ValidateName(name);
        ValidateUserId(userId);

        Name = name;
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
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveBill(Bill bill)
    {
        if (bill is null) throw new ArgumentNullException(nameof(bill));
        if (!_bills.Remove(bill)) throw new InvalidOperationException("Bill not found in the period.");

        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description, DateTime startDate, DateTime endDate)
    {
        UpdateName(name);
        UpdateDescription(description);
        UpdateStartDate(startDate);
        UpdateEndDate(endDate);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStartDate(DateTime newStartDate)
    {
        if (newStartDate > EndDate)
            throw new ArgumentException("StartDate cannot be later than EndDate.", nameof(newStartDate));
        StartDate = newStartDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEndDate(DateTime newEndDate)
    {
        if (newEndDate < StartDate)
            throw new ArgumentException("EndDate cannot be earlier than StartDate.", nameof(newEndDate));
        EndDate = newEndDate;
        UpdatedAt = DateTime.UtcNow;
    }

    private List<Money> CalculateTotalSpent()
    {
        if (_bills.Count == 0)
            return new List<Money>();

        return _bills
            .GroupBy(b => b.TotalPrice.Currency)
            .Select(g => new Money(g.Sum(b => b.TotalPrice.Amount), g.Key))
            .ToList();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Period name cannot be null or empty.", nameof(name));
    }

    private static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("StartDate must be before or equal to EndDate.");
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user identifier.", nameof(userId));
    }
}
