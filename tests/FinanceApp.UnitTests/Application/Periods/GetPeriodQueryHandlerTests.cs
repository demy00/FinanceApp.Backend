using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Periods.Queries;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Periods;

public class GetPeriodQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPeriodResponse_WhenPeriodExists()
    {
        // Arrange
        var periodRepositoryMock = new Mock<IPeriodRepository>();

        var periodId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var expectedResponse = new PeriodResponse(
            periodId,
            "TestPeriod",
            "TestDescription",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(10),
            new List<MoneyDto> { new MoneyDto(100.0m, "USD") }
        );

        periodRepositoryMock
            .Setup(r => r.GetByIdAsync(periodId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var handler = new GetPeriodQueryHandler(periodRepositoryMock.Object);
        var query = new GetPeriodQuery(periodId, userId);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(expectedResponse, result);
        periodRepositoryMock.Verify(r => r.GetByIdAsync(periodId, userId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowPeriodNotFoundException_WhenPeriodDoesNotExist()
    {
        // Arrange
        var periodRepositoryMock = new Mock<IPeriodRepository>();

        var periodId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        periodRepositoryMock
            .Setup(r => r.GetByIdAsync(periodId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as PeriodResponse);

        var handler = new GetPeriodQueryHandler(periodRepositoryMock.Object);
        var query = new GetPeriodQuery(periodId, userId);
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<PeriodNotFoundException>(() =>
            handler.Handle(query, cancellationToken));
    }
}
