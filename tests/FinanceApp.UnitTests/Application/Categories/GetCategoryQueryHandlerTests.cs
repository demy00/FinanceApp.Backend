using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Categories.Queries;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Categories;

public class GetCategoryQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCategoryResponse_WhenCategoryExists()
    {
        // Arrange
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedResponse = new CategoryResponse(
            categoryId,
            "TestCategory",
            "TestDescription"
        );

        categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var handler = new GetCategoryQueryHandler(categoryRepositoryMock.Object);
        var query = new GetCategoryQuery(categoryId, userId);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(expectedResponse, result);
        categoryRepositoryMock.Verify(r => r.GetByIdAsync(categoryId, userId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowCategoryNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryRepositoryMock = new Mock<ICategoryRepository>();

        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as CategoryResponse);

        var handler = new GetCategoryQueryHandler(categoryRepositoryMock.Object);
        var query = new GetCategoryQuery(categoryId, userId);
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
            handler.Handle(query, cancellationToken));
    }
}
