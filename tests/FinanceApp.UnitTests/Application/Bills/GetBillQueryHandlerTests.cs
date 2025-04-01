using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Bills.Queries;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Bills;

public class GetBillQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnBillResponse_WhenBillExists()
    {
        // Arrange
        var billRepositoryMock = new Mock<IBillRepository>();

        var billId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedResponse = new BillResponse(
            billId,
            "TestBill",
            "TestDescription",
            new MoneyDto(100.0m, "USD")
        );

        billRepositoryMock
            .Setup(r => r.GetByIdAsync(billId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var handler = new GetBillQueryHandler(billRepositoryMock.Object);
        var query = new GetBillQuery(billId, userId);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(expectedResponse, result);
        billRepositoryMock.Verify(r => r.GetByIdAsync(billId, userId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBillNotFoundException_WhenBillDoesNotExist()
    {
        // Arrange
        var billRepositoryMock = new Mock<IBillRepository>();

        var billId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        billRepositoryMock
            .Setup(r => r.GetByIdAsync(billId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as BillResponse);

        var handler = new GetBillQueryHandler(billRepositoryMock.Object);
        var query = new GetBillQuery(billId, userId);
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<BillNotFoundException>(() =>
            handler.Handle(query, cancellationToken));
    }
}
