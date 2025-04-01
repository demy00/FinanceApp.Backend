using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Periods.Commands;
using FinanceApp.Domain.Entities;
using Moq;

namespace FinanceApp.UnitTests.Application.Periods;

public class CreatePeriodCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddPeriod_AndSaveChanges()
    {
        // Arrange
        var periodRepoMock = new Mock<IPeriodRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var handler = new CreatePeriodCommandHandler(periodRepoMock.Object, unitOfWorkMock.Object);

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(10);
        var userId = Guid.NewGuid();

        var command = new CreatePeriodCommand("TestName", "TestDescription", startDate, endDate, userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        periodRepoMock.Verify(r => r.Add(It.Is<Period>(p =>
            p.Name == command.Name &&
            p.Description == command.Description &&
            p.StartDate == command.StartDate &&
            p.EndDate == command.EndDate &&
            p.UserId == command.UserId)), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }
}