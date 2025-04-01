using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Periods.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Periods;

public class DeletePeriodCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeletePeriod_AndSaveChanges_WhenPeriodExists()
    {
        // Arrange
        var periodRepoMock = new Mock<IPeriodRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var periodId = Guid.NewGuid();
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(10);
        var userId = Guid.NewGuid();

        var period = new Period("TestName", "TestDescription", startDate, endDate, userId);

        periodRepoMock
            .Setup(r => r.GetDomainByIdAsync(periodId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(period);

        var handler = new DeletePeriodCommandHandler(periodRepoMock.Object, unitOfWorkMock.Object);

        var command = new DeletePeriodCommand(periodId, userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        periodRepoMock.Verify(r => r.Remove(period), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowPeriodNotFoundException_WhenPeriodDoesNotExist()
    {
        // Arrange
        var periodRepoMock = new Mock<IPeriodRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var periodId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        periodRepoMock
            .Setup(r => r.GetDomainByIdAsync(periodId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Period);

        var handler = new DeletePeriodCommandHandler(periodRepoMock.Object, unitOfWorkMock.Object);

        var command = new DeletePeriodCommand(periodId, userId);

        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<PeriodNotFoundException>(() => handler.Handle(command, cancellationToken));
    }
}
