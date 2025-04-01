using FinanceApp.Domain.Entities;

namespace FinanceApp.UnitTests.Domain.Entities;

public class CategoryTests
{
    [Fact]
    public void CreateUserCategory_ShouldCreateCategoryWithUserId()
    {
        var userId = Guid.NewGuid();
        var name = "Work Expenses";
        var description = "Expenses related to work";

        var category = new Category(name, description, userId);

        Assert.Equal(name, category.Name);
        Assert.Equal(description, category.Description);
        Assert.Equal(userId, category.UserId);
    }

    [Fact]
    public void CreatePredefinedCategory_ShouldCreateCategoryWithoutUserId()
    {
        var predefinedId = Guid.NewGuid();
        var name = "Groceries";
        var description = "Essential food items";

        var category = new Category(predefinedId, name, description, null);

        Assert.Equal(name, category.Name);
        Assert.Equal(description, category.Description);
        Assert.Null(category.UserId);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsEmpty()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Category("", "Test Description", Guid.NewGuid()));
        Assert.Contains("Category name is required.", ex.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsWhitespace()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Category("   ", "Test Description", Guid.NewGuid()));
        Assert.Contains("Category name is required.", ex.Message);
    }

    [Fact]
    public void Rename_ShouldUpdateName_WhenCategoryIsUserDefined()
    {
        var userId = Guid.NewGuid();
        var category = new Category("Old Name", "Test description", userId);

        category.Rename("New Name");

        Assert.Equal("New Name", category.Name);
    }

    [Fact]
    public void Rename_ShouldThrowException_WhenCategoryIsPredefined()
    {
        var category = new Category(Guid.NewGuid(), "Groceries", "Predefined Category", null);

        var ex = Assert.Throws<InvalidOperationException>(() => category.Rename("New Name"));
        Assert.Equal("Predefined categories cannot be modified.", ex.Message);
    }

    [Fact]
    public void Rename_ShouldThrowException_WhenNewNameIsEmpty()
    {
        var userId = Guid.NewGuid();
        var category = new Category("Old Name", "Test description", userId);

        var ex = Assert.Throws<ArgumentException>(() => category.Rename(""));
        Assert.Contains("Category name is required.", ex.Message);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription_WhenCategoryIsUserDefined()
    {
        var userId = Guid.NewGuid();
        var category = new Category("Work Expenses", "Initial description", userId);

        category.UpdateDescription("Updated description");

        Assert.Equal("Updated description", category.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldThrowException_WhenCategoryIsPredefined()
    {
        var category = new Category(Guid.NewGuid(), "Groceries", "Predefined Category", null);

        var ex = Assert.Throws<InvalidOperationException>(() => category.UpdateDescription("New Description"));
        Assert.Equal("Predefined categories cannot be modified.", ex.Message);
    }

    [Fact]
    public void DefaultCategories_ShouldContainExpectedNumberOfCategories()
    {
        var categories = DefaultCategories.List().ToList();

        Assert.Equal(4, categories.Count);
    }

    [Theory]
    [InlineData("11111111-1111-1111-1111-111111111111", "Groceries", "Essential food and daily consumables")]
    [InlineData("22222222-2222-2222-2222-222222222222", "Utilities", "Electricity, water, gas, and related services")]
    [InlineData("33333333-3333-3333-3333-333333333333", "Entertainment", "Movies, go outs, and leisure activities")]
    [InlineData("44444444-4444-4444-4444-444444444444", "Other", "")]
    public void DefaultCategories_ShouldHaveExpectedValues(string id, string expectedName, string expectedDescription)
    {
        var category = DefaultCategories.List().FirstOrDefault(c => c.Id == Guid.Parse(id));

        Assert.NotNull(category);
        Assert.Equal(expectedName, category.Name);
        Assert.Equal(expectedDescription, category.Description);
        Assert.Null(category.UserId);
    }

    [Fact]
    public void DefaultCategories_ShouldBeImmutable()
    {
        var category = DefaultCategories.Groceries;

        Assert.Throws<InvalidOperationException>(() => category.Rename("New Name"));
        Assert.Throws<InvalidOperationException>(() => category.UpdateDescription("New Description"));
    }
}
