using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Entities;

public class Bill : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money TotalPrice => CalculateTotalPrice();

    private readonly List<BillItem> _items = new();
    public IReadOnlyCollection<BillItem> BillItems => _items.AsReadOnly();
    public Guid UserId { get; init; }

#pragma warning disable CS8618 // Required for EF Core
    private Bill() { }
#pragma warning restore CS8618

    public Bill(string name, string description, Guid userId)
    {
        ValidateName(name);
        ValidateUserId(userId);

        Name = name;
        Description = description ?? string.Empty;
        UserId = userId;
    }

    public void AddBillItems(IEnumerable<BillItem> billItems)
    {
        if (billItems is null)
            throw new ArgumentNullException(nameof(billItems));

        foreach (var item in billItems)
        {
            AddBillItem(item);
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddBillItem(BillItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (_items.Contains(item)) throw new InvalidOperationException("Item already exists in the bill.");
        if (_items.Count > 0 && item.Price.Currency != _items.First().Price.Currency)
            throw new InvalidOperationException("All items in a bill must have the same currency.");

        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(BillItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (!_items.Remove(item)) throw new InvalidOperationException("Item not found in the bill.");

        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description)
    {
        UpdateName(name);
        UpdateDescription(description);
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

    private Money CalculateTotalPrice()
    {
        if (_items.Count == 0) return new Money(0, "");

        decimal amount = _items.Sum(i => i.Price.Amount * i.Quantity.Value);
        string currency = _items.First().Price.Currency;

        return new Money(amount, currency);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Bill name cannot be null or empty.", nameof(name));
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user identifier.", nameof(userId));
    }
}
