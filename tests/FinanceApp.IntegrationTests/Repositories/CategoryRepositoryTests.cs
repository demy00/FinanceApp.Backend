using FinanceApp.Domain.Entities;
using FinanceApp.Infrastructure.Repositories;

namespace FinanceApp.Infrastructure.Tests.Repositories;

public class CategoryRepositoryTests : TestBase
{
    private readonly CategoryRepository _categoryRepository;

    private readonly Guid _testUserId = Guid.NewGuid();

    public CategoryRepositoryTests()
    {
        _categoryRepository = new CategoryRepository(Context);
    }

    [Fact]
    public async Task Add_Should_Add_Category()
    {
        // Arrange
        var category = new Category("Test", "Test Description", _testUserId);

        // Act
        _categoryRepository.Add(category);
        await Context.SaveChangesAsync();

        // Assert
        var retrieved = await _categoryRepository.GetDomainByIdAsync(category.Id, _testUserId, CancellationToken.None);
        Assert.NotNull(retrieved);
        Assert.Equal("Test", retrieved.Name);
        Assert.Equal("Test Description", retrieved.Description);
    }

    [Fact]
    public async Task GetDomainByIdAsync_Should_Return_Null_For_Wrong_User()
    {
        // Arrange
        var category = new Category("Test", "Test Description", _testUserId);
        _categoryRepository.Add(category);
        await Context.SaveChangesAsync();

        // Act
        var result = await _categoryRepository.GetDomainByIdAsync(category.Id, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_CategoryResponse()
    {
        // Arrange
        var category = new Category("Test", "Test Description", _testUserId);
        _categoryRepository.Add(category);
        await Context.SaveChangesAsync();

        // Act
        var result = await _categoryRepository.GetByIdAsync(category.Id, _testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("Test", result.Name);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task GetAsync_Should_Return_All_Categories_With_Pagination_And_Sorting()
    {
        // Arrange
        var sampleCategories = TestDataHelper.GetSampleCategories(_testUserId);
        foreach (var category in sampleCategories)
        {
            _categoryRepository.Add(category);
        }
        await Context.SaveChangesAsync();

        // Act
        var result = await _categoryRepository.GetAsync(
            _testUserId,
            searchTerm: null,
            sortColumn: "name",
            sortOrder: "asc",
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.TotalCount);

        var expectedOrder = sampleCategories.OrderBy(c => c.Name).Select(c => c.Name).ToList();
        var actualOrder = result.Items.Select(r => r.Name).ToList();
        Assert.Equal(expectedOrder, actualOrder);
    }

    [Fact]
    public async Task GetAsync_Should_Filter_By_SearchTerm()
    {
        // Arrange
        var sampleCategories = TestDataHelper.GetSampleCategories(_testUserId);
        foreach (var category in sampleCategories)
        {
            _categoryRepository.Add(category);
        }
        await Context.SaveChangesAsync();

        // Act
        var result = await _categoryRepository.GetAsync(
            _testUserId,
            searchTerm: "Food",
            sortColumn: "name",
            sortOrder: "asc",
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Food", result.Items.First().Name);
    }

    [Fact]
    public async Task Remove_Should_Delete_Category()
    {
        // Arrange
        var category = new Category("Test", "Test Description", _testUserId);
        _categoryRepository.Add(category);
        await Context.SaveChangesAsync();

        // Act
        _categoryRepository.Remove(category);
        await Context.SaveChangesAsync();

        // Assert
        var retrieved = await _categoryRepository.GetDomainByIdAsync(category.Id, _testUserId, CancellationToken.None);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task Update_Should_Modify_Category()
    {
        // Arrange
        var category = new Category("Test", "Test Description", _testUserId);
        _categoryRepository.Add(category);
        await Context.SaveChangesAsync();

        // Act
        category.Update("Updated Name", "Updated Description");
        _categoryRepository.Update(category);
        await Context.SaveChangesAsync();

        // Assert
        var updatedCategory = await _categoryRepository.GetDomainByIdAsync(category.Id, _testUserId, CancellationToken.None);
        Assert.NotNull(updatedCategory);
        Assert.Equal("Updated Name", updatedCategory.Name);
        Assert.Equal("Updated Description", updatedCategory.Description);
    }
}

