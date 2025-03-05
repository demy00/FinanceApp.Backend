using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Entities;

public class BillItem : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Category? Category { get; private set; }
    public Money Price { get; private set; }
    public Quantity Quantity { get; private set; }
    public Guid UserId { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private BillItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public BillItem(string name, string description, Category category, Money price, Quantity quantity, Guid userId)
    {
        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        Description = description ?? string.Empty;
        Category = category ?? DefaultCategories.Other;
        Price = price ?? new Money(0, "");
        Quantity = quantity ?? new Quantity(1);
        UserId = userId;
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

    public void UpdateCategory(Category category)
    {
        Category = category ?? DefaultCategories.Other;
    }
}
