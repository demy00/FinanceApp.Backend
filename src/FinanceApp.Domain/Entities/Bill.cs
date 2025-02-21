using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Entities;

public class Bill : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money TotalPrice => GetTotalPrice();

    private readonly List<BillItem> _items = new();
    public IReadOnlyCollection<BillItem> Items => _items.AsReadOnly();

    public Bill(Guid id, string name, string description)
    {
        Id = id;
        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        Description = description ?? string.Empty;
    }

    public void AddItem(BillItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (_items.Contains(item)) throw new InvalidOperationException("Item already exists in the bill.");
        if (_items.Count > 0 && item.Price.Currency != _items.First().Price.Currency)
            throw new InvalidOperationException("All items in a bill must have the same currency.");

        _items.Add(item);
    }

    public void RemoveItem(BillItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (!_items.Remove(item)) throw new InvalidOperationException("Item not found in the bill.");
    }

    public void UpdateName(string name)
    {
        Name = name ?? string.Empty;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
    }

    private Money GetTotalPrice()
    {
        if (_items.Count == 0) return new Money(0, "");

        decimal amount = _items.Sum(i => i.TotalPrice.Amount);
        string currency = _items.First().TotalPrice.Currency;

        return new Money(amount, currency);
    }
}
