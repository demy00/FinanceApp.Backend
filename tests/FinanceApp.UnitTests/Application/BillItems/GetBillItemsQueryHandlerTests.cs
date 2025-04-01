using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Bills.Queries;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using Moq;

namespace FinanceApp.UnitTests.Application.BillItems;

public class GetBillItemsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPageListOfBillResponse()
    {
        // Arrange
        var billRepositoryMock = new Mock<IBillRepository>();

        var userId = Guid.NewGuid();
        var query = new GetBillsQuery(
            UserId: userId,
            SearchTerm: "Test",
            SortColumn: "Name",
            SortOrder: "asc",
            Page: 1,
            PageSize: 10);

        var cancellationToken = new CancellationToken();

        var expectedBills = new List<BillResponse>
            {
                new BillResponse(
                    Id: Guid.NewGuid(),
                    Name: "Test Bill",
                    Description: "Test Description",
                    new MoneyDto(100m, "USD"))
            };

        var expectedPageList = new PageList<BillResponse>(
            items: expectedBills,
            page: query.Page,
            pageSize: query.PageSize,
            totalCount: expectedBills.Count);

        billRepositoryMock
            .Setup(r => r.GetAsync(
                query.UserId,
                query.SearchTerm,
                query.SortColumn,
                query.SortOrder,
                query.Page,
                query.PageSize,
                cancellationToken))
            .ReturnsAsync(expectedPageList);

        var handler = new GetBillsQueryHandler(billRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(expectedPageList, result);
        billRepositoryMock.Verify(r => r.GetAsync(
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
