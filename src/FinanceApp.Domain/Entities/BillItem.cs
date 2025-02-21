using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Entities;

public class BillItem : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money TotalPrice => new Money(Price.Amount * Quantity.Value, Price.Currency);

    public BillItem(Guid id, string name, string description, Money price, Quantity quantity)
    {
        Id = id;
        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        Description = description ?? string.Empty;
        Price = price ?? new Money(0, "");
        Quantity = quantity ?? new Quantity(1);
    }

    public void UpdateName(string name)
    {
        Name = name ?? string.Empty;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice is null) throw new ArgumentNullException(nameof(newPrice));
        Price = newPrice;
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        if (newQuantity is null) throw new ArgumentNullException(nameof(newQuantity));
        Quantity = newQuantity;
    }
}
