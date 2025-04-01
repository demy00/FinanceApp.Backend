using FinanceApp.Domain.ValueObjects;
using FinanceApp.Infrastructure.Repositories;

namespace FinanceApp.Infrastructure.Tests.Repositories;

public class BillItemRepositoryTests : TestBase
{
    private readonly BillItemRepository _billItemRepository;
    private readonly Guid _testUserId = Guid.NewGuid();

    public BillItemRepositoryTests()
    {
        _billItemRepository = new BillItemRepository(Context);
    }

    [Fact]
    public async Task Add_Should_Add_BillItem()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItem = TestDataHelper.GetSampleBillItems(_testUserId, categories).First();

        // Act
        _billItemRepository.Add(billItem);
        await Context.SaveChangesAsync();

        // Assert
        var retrieved = await _billItemRepository.GetDomainByIdAsync(billItem.Id, _testUserId, CancellationToken.None);
        Assert.NotNull(retrieved);
        Assert.Equal(billItem.Name, retrieved.Name);
        Assert.Equal(billItem.Description, retrieved.Description);
    }

    [Fact]
    public async Task GetDomainByIdAsync_Should_Return_Null_For_Wrong_User()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItem = TestDataHelper.GetSampleBillItems(_testUserId, categories).First();
        _billItemRepository.Add(billItem);
        await Context.SaveChangesAsync();

        // Act
        var result = await _billItemRepository.GetDomainByIdAsync(billItem.Id, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_BillItemResponse()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItem = TestDataHelper.GetSampleBillItems(_testUserId, categories).First();
        _billItemRepository.Add(billItem);
        await Context.SaveChangesAsync();

        // Act
        var response = await _billItemRepository.GetByIdAsync(billItem.Id, _testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(billItem.Id, response.Id);
        Assert.Equal(billItem.Name, response.Name);
        Assert.Equal(billItem.Description, response.Description);
        Assert.Equal(billItem.Category.Name, response.Category.Name);
        Assert.Equal(billItem.Category.Description, response.Category.Description);
        Assert.Equal(billItem.Price.Amount, response.Price.Amount);
        Assert.Equal(billItem.Price.Currency, response.Price.Currency);
        Assert.Equal(billItem.Quantity.Value, response.Quantity.Value);
    }

    [Fact]
    public async Task GetAsync_Should_Return_All_BillItems_With_Pagination_And_Sorting()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        foreach (var item in billItems)
        {
            _billItemRepository.Add(item);
        }
        await Context.SaveChangesAsync();

        // Act
        var pageList = await _billItemRepository.GetAsync(
            _testUserId,
            searchTerm: null,
            sortColumn: "name",
            sortOrder: "asc",
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.NotNull(pageList);
        Assert.Equal(billItems.Count, pageList.TotalCount);
        var expectedOrder = billItems.OrderBy(bi => bi.Name).Select(bi => bi.Name).ToList();
        var actualOrder = pageList.Items.Select(bi => bi.Name).ToList();
        Assert.Equal(expectedOrder, actualOrder);
    }

    [Fact]
    public async Task GetAsync_Should_Filter_By_SearchTerm_Currency()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        foreach (var item in billItems)
        {
            _billItemRepository.Add(item);
        }
        await Context.SaveChangesAsync();

        // Act
        var pageList = await _billItemRepository.GetAsync(
            _testUserId,
            searchTerm: "currency_USD",
            sortColumn: "name",
            sortOrder: "asc",
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.NotNull(pageList);
        Assert.All(pageList.Items, item => Assert.Equal("USD", item.Price.Currency));
    }

    [Fact]
    public async Task Remove_Should_Delete_BillItem()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItem = TestDataHelper.GetSampleBillItems(_testUserId, categories).First();
        _billItemRepository.Add(billItem);
        await Context.SaveChangesAsync();

        // Act
        _billItemRepository.Remove(billItem);
        await Context.SaveChangesAsync();

        // Assert
        var retrieved = await _billItemRepository.GetDomainByIdAsync(billItem.Id, _testUserId, CancellationToken.None);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task Update_Should_Modify_BillItem()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItem = TestDataHelper.GetSampleBillItems(_testUserId, categories).First();
        _billItemRepository.Add(billItem);
        await Context.SaveChangesAsync();

        // Act
        billItem.UpdateName("Updated Name");
        billItem.UpdatePrice(new Money(99.99m, "USD"));
        _billItemRepository.Update(billItem);
        await Context.SaveChangesAsync();

        // Assert
        var updated = await _billItemRepository.GetDomainByIdAsync(billItem.Id, _testUserId, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal(99.99m, updated.Price.Amount);
        Assert.Equal("USD", updated.Price.Currency);
    }
}
