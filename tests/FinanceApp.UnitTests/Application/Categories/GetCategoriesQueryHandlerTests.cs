using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Categories.Queries;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using Moq;

namespace FinanceApp.UnitTests.Application.Categories;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPageListOfCategoryResponse()
    {
        // Arrange
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var userId = Guid.NewGuid();
        var query = new GetCategoriesQuery(
            UserId: userId,
            SearchTerm: "Test",
            SortColumn: "Name",
            SortOrder: "asc",
            Page: 1,
            PageSize: 10);

        var cancellationToken = new CancellationToken();

        var expectedCategorys = new List<CategoryResponse>
            {
                new CategoryResponse(
                    Id: Guid.NewGuid(),
                    Name: "Test Category",
                    Description: "Test Description")
            };

        var expectedPageList = new PageList<CategoryResponse>(
            items: expectedCategorys,
            page: query.Page,
            pageSize: query.PageSize,
            totalCount: expectedCategorys.Count);

        categoryRepositoryMock
            .Setup(r => r.GetAsync(
                query.UserId,
                query.SearchTerm,
                query.SortColumn,
                query.SortOrder,
                query.Page,
                query.PageSize,
                cancellationToken))
            .ReturnsAsync(expectedPageList);

        var handler = new GetCategoriesQueryHandler(categoryRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(expectedPageList, result);
        categoryRepositoryMock.Verify(r => r.GetAsync(
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
