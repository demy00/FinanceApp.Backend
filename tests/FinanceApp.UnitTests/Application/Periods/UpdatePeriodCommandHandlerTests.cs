using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Periods.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Periods;

public class UpdatePeriodCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdatePeriod_AndSaveChanges()
    {
        // Arrange
        var periodRepoMock = new Mock<IPeriodRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var periodId = Guid.NewGuid();
        var oldStartDate = DateTime.UtcNow;
        var oldEndDate = oldStartDate.AddDays(10);
        var userId = Guid.NewGuid();

        var period = new Period("OldName", "OldDescription", oldStartDate, oldEndDate, userId);

        periodRepoMock
            .Setup(r => r.GetDomainByIdAsync(periodId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(period);

        var handler = new UpdatePeriodCommandHandler(periodRepoMock.Object, unitOfWorkMock.Object);

        var newStartDate = DateTime.UtcNow.AddDays(1);
        var newEndDate = newStartDate.AddDays(10);
        var command = new UpdatePeriodCommand(
            periodId,
            "NewName",
            "NewDescription",
            newStartDate,
            newEndDate,
            userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal("NewName", period.Name);
        Assert.Equal("NewDescription", period.Description);
        Assert.Equal(newStartDate, period.StartDate);
        Assert.Equal(newEndDate, period.EndDate);

        periodRepoMock.Verify(r => r.Update(period), Times.Once);

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

        var handler = new UpdatePeriodCommandHandler(periodRepoMock.Object, unitOfWorkMock.Object);

        var newStartDate = DateTime.UtcNow.AddDays(1);
        var newEndDate = newStartDate.AddDays(10);

        var command = new UpdatePeriodCommand(
            periodId,
            "NewName",
            "NewDescription",
            newStartDate,
            newEndDate,
            userId);

        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<PeriodNotFoundException>(() => handler.Handle(command, cancellationToken));
    }
}
