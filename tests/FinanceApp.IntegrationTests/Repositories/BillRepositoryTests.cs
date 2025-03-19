using FinanceApp.Domain.Entities;
using FinanceApp.Infrastructure.Repositories;

namespace FinanceApp.Infrastructure.Tests.Repositories;

public class BillRepositoryTests : TestBase
{
    private readonly BillRepository _billRepository;
    private readonly Guid _testUserId = Guid.NewGuid();

    public BillRepositoryTests()
    {
        _billRepository = new BillRepository(Context);
    }

    [Fact]
    public async Task Add_Should_Add_Bill()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        var bill = new Bill("Test Bill", "Test Description", _testUserId);
        bill.AddBillItem(billItems.First());
        bill.AddBillItem(billItems.Skip(1).First());

        // Act
        _billRepository.Add(bill);
        await Context.SaveChangesAsync();

        // Assert
        var retrieved = await _billRepository.GetDomainByIdAsync(bill.Id, _testUserId, CancellationToken.None);
        Assert.NotNull(retrieved);
        Assert.Equal(bill.Name, retrieved.Name);
        Assert.Equal(bill.Description, retrieved.Description);
        Assert.NotEmpty(retrieved.BillItems);
    }

    [Fact]
    public async Task GetDomainByIdAsync_Should_Return_Null_For_Wrong_User()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        var bill = new Bill("Test Bill", "Test Description", _testUserId);
        bill.AddBillItem(billItems.First());
        _billRepository.Add(bill);
        await Context.SaveChangesAsync();

        // Act
        var result = await _billRepository.GetDomainByIdAsync(bill.Id, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_BillResponse()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        var bill = new Bill("Test Bill", "Test Description", _testUserId);
        bill.AddBillItem(billItems[0]); // Amount: 3 USD
        bill.AddBillItem(billItems[1]); // Amount: 15 USD
        _billRepository.Add(bill);
        await Context.SaveChangesAsync();

        // Act
        var response = await _billRepository.GetByIdAsync(bill.Id, _testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(bill.Id, response.Id);
        Assert.Equal("Test Bill", response.Name);
        Assert.Equal("Test Description", response.Description);
        Assert.Equal(18m, response.TotalPrice.Amount);
        Assert.Equal("USD", response.TotalPrice.Currency);
    }

    [Fact]
    public async Task GetAsync_Should_Return_All_Bills_With_Pagination_And_Sorting()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        var bills = TestDataHelper.GetSampleBills(_testUserId, billItems);
        foreach (var bill in bills)
        {
            _billRepository.Add(bill);
        }
        await Context.SaveChangesAsync();

        // Act
        var pageList = await _billRepository.GetAsync(
            _testUserId,
            searchTerm: null,
            sortColumn: "name",
            sortOrder: "asc",
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.NotNull(pageList);
        Assert.Equal(bills.Count, pageList.TotalCount);
        var expectedOrder = bills.OrderBy(b => b.Name).Select(b => b.Name).ToList();
        var actualOrder = pageList.Items.Select(b => b.Name).ToList();
        Assert.Equal(expectedOrder, actualOrder);
    }

    [Fact]
    public async Task GetAsync_Should_Filter_By_SearchTerm_Currency()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        var bills = TestDataHelper.GetSampleBills(_testUserId, billItems);
        foreach (var bill in bills)
        {
            _billRepository.Add(bill);
        }
        await Context.SaveChangesAsync();

        // Act
        var pageList = await _billRepository.GetAsync(
            _testUserId,
            searchTerm: "currency_USD",
            sortColumn: "name",
            sortOrder: "asc",
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.NotNull(pageList);
        Assert.All(pageList.Items, billResponse =>
        {
            Assert.Equal("USD", billResponse.TotalPrice.Currency);
        });
    }

    [Fact]
    public async Task Remove_Should_Delete_Bill()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        var bill = new Bill("Test Bill", "Test Description", _testUserId);
        bill.AddBillItem(billItems.First());
        _billRepository.Add(bill);
        await Context.SaveChangesAsync();

        // Act
        _billRepository.Remove(bill);
        await Context.SaveChangesAsync();

        // Assert
        var retrieved = await _billRepository.GetDomainByIdAsync(bill.Id, _testUserId, CancellationToken.None);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task Update_Should_Modify_Bill()
    {
        // Arrange
        var categories = TestDataHelper.GetSampleCategories(_testUserId);
        var billItems = TestDataHelper.GetSampleBillItems(_testUserId, categories);
        var bill = new Bill("Test Bill", "Test Description", _testUserId);
        bill.AddBillItem(billItems.First());
        _billRepository.Add(bill);
        await Context.SaveChangesAsync();

        // Act
        bill.UpdateName("Updated Bill");
        bill.UpdateDescription("Updated Description");
        _billRepository.Update(bill);
        await Context.SaveChangesAsync();

        // Assert
        var updated = await _billRepository.GetDomainByIdAsync(bill.Id, _testUserId, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.Equal("Updated Bill", updated.Name);
        Assert.Equal("Updated Description", updated.Description);
    }
}
