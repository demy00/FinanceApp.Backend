using FinanceApp.Application.Abstractions;
using FinanceApp.Application.BillItems.Queries;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.BillItems;

public class GetBillItemQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnBillItemResponse_WhenBillItemExists()
    {
        // Arrange
        var billItemRepositoryMock = new Mock<IBillItemRepository>();

        var billItemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedResponse = new BillItemResponse(
            billItemId,
            "TestBill",
            "TestDescription",
            new CategoryDto("TestName", "TestDescription"),
            new MoneyDto(100, "USD"),
            new QuantityDto(1)
        );

        billItemRepositoryMock
            .Setup(r => r.GetByIdAsync(billItemId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var handler = new GetBillItemQueryHandler(billItemRepositoryMock.Object);
        var query = new GetBillItemQuery(billItemId, userId);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(expectedResponse, result);
        billItemRepositoryMock.Verify(r => r.GetByIdAsync(billItemId, userId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBillItemNotFoundException_WhenBillItemDoesNotExist()
    {
        // Arrange
        var billItemRepositoryMock = new Mock<IBillItemRepository>();

        var billItemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        billItemRepositoryMock
            .Setup(r => r.GetByIdAsync(billItemId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as BillItemResponse);

        var handler = new GetBillItemQueryHandler(billItemRepositoryMock.Object);
        var query = new GetBillItemQuery(billItemId, userId);
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<BillItemNotFoundException>(() =>
            handler.Handle(query, cancellationToken));
    }
}
