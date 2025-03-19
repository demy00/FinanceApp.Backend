using FinanceApp.Infrastructure.Repositories;

namespace FinanceApp.Infrastructure.Tests.Repositories;

public class PeriodRepositoryTests : TestBase
{
    private readonly PeriodRepository _periodRepository;
    private readonly Guid _testUserId = Guid.NewGuid();

    public PeriodRepositoryTests()
    {
        _periodRepository = new PeriodRepository(Context);
    }

    [Fact]
    public async Task Add_Should_Add_Period()
    {
        // Arrange
        var period = TestDataHelper.GetSamplePeriods(_testUserId).First();

        // Act
        _periodRepository.Add(period);
        await Context.SaveChangesAsync();

        // Assert
        var retrieved = await _periodRepository.GetDomainByIdAsync(period.Id, _testUserId, CancellationToken.None);
        Assert.NotNull(retrieved);
        Assert.Equal(period.Name, retrieved.Name);
        Assert.Equal(period.Description, retrieved.Description);
        Assert.NotEmpty(retrieved.Bills);
        Assert.All(retrieved.Bills, b => Assert.NotEmpty(b.BillItems));
    }

    [Fact]
    public async Task GetDomainByIdAsync_Should_Return_Null_For_Wrong_User()
    {
        // Arrange
        var period = TestDataHelper.GetSamplePeriods(_testUserId).First();
        _periodRepository.Add(period);
        await Context.SaveChangesAsync();

        // Act
        var result = await _periodRepository.GetDomainByIdAsync(period.Id, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_PeriodResponse()
    {
        // Arrange
        var period = TestDataHelper.GetSamplePeriods(_testUserId).First();
        _periodRepository.Add(period);
        await Context.SaveChangesAsync();

        // Act
        var response = await _periodRepository.GetByIdAsync(period.Id, _testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(period.Id, response.Id);
        Assert.Equal(period.Name, response.Name);
        Assert.Equal(period.Description, response.Description);
        Assert.Equal(period.StartDate, response.StartDate);
        Assert.Equal(period.EndDate, response.EndDate);

        Assert.NotNull(response.TotalSpent);
        Assert.Equal(2, response.TotalSpent.Count);
        var usdTotal = response.TotalSpent.FirstOrDefault(m => m.Currency == "USD")?.Amount;
        var eurTotal = response.TotalSpent.FirstOrDefault(m => m.Currency == "EUR")?.Amount;
        Assert.Equal(18m, usdTotal);
        Assert.Equal(14m, eurTotal);
    }

    [Fact]
    public async Task GetAsync_Should_Return_All_Periods_With_Pagination_And_Sorting()
    {
        // Arrange
        var periods = TestDataHelper.GetSamplePeriods(_testUserId);
        foreach (var period in periods)
        {
            _periodRepository.Add(period);
        }
        await Context.SaveChangesAsync();

        // Act
        var pageList = await _periodRepository.GetAsync(
            _testUserId,
            searchTerm: null,
            sortColumn: "name",
            sortOrder: "asc",
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.NotNull(pageList);
        Assert.Equal(periods.Count, pageList.TotalCount);
        var expectedOrder = periods.OrderBy(p => p.Name).Select(p => p.Name).ToList();
        var actualOrder = pageList.Items.Select(p => p.Name).ToList();
        Assert.Equal(expectedOrder, actualOrder);
    }

    [Fact]
    public async Task GetAsync_Should_Filter_By_SearchTerm_Currency()
    {
        // Arrange
        var periods = TestDataHelper.GetSamplePeriods(_testUserId);
        foreach (var period in periods)
        {
            _periodRepository.Add(period);
        }
        await Context.SaveChangesAsync();

        // Act
        var pageList = await _periodRepository.GetAsync(
            _testUserId,
            searchTerm: "currency_USD",
            sortColumn: "name",
            sortOrder: "asc",
            page: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        Assert.NotNull(pageList);
        Assert.All(pageList.Items, periodResponse =>
        {
            Assert.Contains(periodResponse.TotalSpent, m => m.Currency == "USD");
        });
    }

    [Fact]
    public async Task Remove_Should_Delete_Period()
    {
        // Arrange
        var period = TestDataHelper.GetSamplePeriods(_testUserId).First();
        _periodRepository.Add(period);
        await Context.SaveChangesAsync();

        // Act
        _periodRepository.Remove(period);
        await Context.SaveChangesAsync();

        // Assert
        var retrieved = await _periodRepository.GetDomainByIdAsync(period.Id, _testUserId, CancellationToken.None);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task Update_Should_Modify_Period()
    {
        // Arrange
        var period = TestDataHelper.GetSamplePeriods(_testUserId).First();
        _periodRepository.Add(period);
        await Context.SaveChangesAsync();

        // Act
        period.UpdateName("Updated Period");
        period.UpdateDescription("Updated Description");
        _periodRepository.Update(period);
        await Context.SaveChangesAsync();

        // Assert
        var updated = await _periodRepository.GetDomainByIdAsync(period.Id, _testUserId, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.Equal("Updated Period", updated.Name);
        Assert.Equal("Updated Description", updated.Description);
    }
}
