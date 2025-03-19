using FinanceApp.Domain.ValueObjects;

namespace FinanceApp.Domain.Entities;

public class BillItem : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Category Category { get; private set; }
    public Money Price { get; private set; }
    public Quantity Quantity { get; private set; }
    public Guid UserId { get; init; }

#pragma warning disable CS8618  // Required for EF Core.
    private BillItem() { }
#pragma warning restore CS8618

    public BillItem(string name, string description, Category category, Money price, Quantity quantity, Guid userId)
        : base() // Uses BaseEntity's default Id generation.
    {
        ValidateName(name);
        ValidateUserId(userId);

        Name = name;
        Description = description ?? string.Empty;
        Category = category ?? DefaultCategories.Other;
        Price = price ?? new Money(0, "");
        Quantity = quantity ?? new Quantity(1);
        UserId = userId;
    }

    public void Update(
        string name,
        string description,
        Category category,
        Money newPrice,
        Quantity newQuantity)
    {
        UpdateName(name);
        UpdateDescription(description);
        UpdateCategory(category);
        UpdatePrice(newPrice);
        UpdateQuantity(newQuantity);
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

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice is null)
            throw new ArgumentNullException(nameof(newPrice));
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        if (newQuantity is null)
            throw new ArgumentNullException(nameof(newQuantity));
        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCategory(Category category)
    {
        Category = category ?? DefaultCategories.Other;
        UpdatedAt = DateTime.UtcNow;
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
