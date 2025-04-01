using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.Application.Periods.Queries;
using Moq;

namespace FinanceApp.UnitTests.Application.Periods;

public class GetPeriodsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPageListOfPeriodResponse()
    {
        // Arrange
        var periodRepositoryMock = new Mock<IPeriodRepository>();

        var userId = Guid.NewGuid();
        var query = new GetPeriodsQuery(
            UserId: userId,
            SearchTerm: "Test",
            SortColumn: "Name",
            SortOrder: "asc",
            Page: 1,
            PageSize: 10);

        var cancellationToken = new CancellationToken();

        var expectedPeriods = new List<PeriodResponse>
            {
                new PeriodResponse(
                    Id: Guid.NewGuid(),
                    Name: "Test Period",
                    Description: "Test Description",
                    StartDate: DateTime.UtcNow,
                    EndDate: DateTime.UtcNow.AddDays(1),
                    TotalSpent: new List<MoneyDto> { new MoneyDto(100m, "USD") })
            };

        var expectedPageList = new PageList<PeriodResponse>(
            items: expectedPeriods,
            page: query.Page,
            pageSize: query.PageSize,
            totalCount: expectedPeriods.Count);

        periodRepositoryMock
            .Setup(r => r.GetAsync(
                query.UserId,
                query.SearchTerm,
                query.SortColumn,
                query.SortOrder,
                query.Page,
                query.PageSize,
                cancellationToken))
            .ReturnsAsync(expectedPageList);

        var handler = new GetPeriodsQueryHandler(periodRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(expectedPageList, result);
        periodRepositoryMock.Verify(r => r.GetAsync(
                query.UserId,
                query.SearchTerm,
                query.SortColumn,
                query.SortOrder,
                query.Page,
                query.PageSize,
                cancellationToken),
            Times.Once);
    }
}
