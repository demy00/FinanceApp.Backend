namespace FinanceApp.Domain.Entities;

public static class DefaultCategories
{
    public static Category Groceries { get; } = new Category(
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        "Groceries",
        "Essential food and daily consumables",
        null);

    public static Category Utilities { get; } = new Category(
        Guid.Parse("22222222-2222-2222-2222-222222222222"),
        "Utilities",
        "Electricity, water, gas, and related services",
        null);

    public static Category Entertainment { get; } = new Category(
        Guid.Parse("33333333-3333-3333-3333-333333333333"),
        "Entertainment",
        "Movies, go outs, and leisure activities",
        null);

    public static Category Other { get; } = new Category(
        Guid.Parse("44444444-4444-4444-4444-444444444444"),
        "Other",
        "",
        null);

    public static IEnumerable<Category> List() => new List<Category>
        {
            Groceries,
            Utilities,
            Entertainment,
            Other
        };
}