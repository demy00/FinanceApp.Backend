using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Bills.Commands;
using FinanceApp.Domain.Entities;
using Moq;

namespace FinanceApp.UnitTests.Application.Bills;

public class CreateBillCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddBill_AndSaveChanges()
    {
        // Arrange
        var billRepoMock = new Mock<IBillRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var handler = new CreateBillCommandHandler(billRepoMock.Object, unitOfWorkMock.Object);

        var userId = Guid.NewGuid();
        var command = new CreateBillCommand("TestName", "TestDescription", userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        billRepoMock.Verify(r => r.Add(It.Is<Bill>(b =>
            b.Name == command.Name &&
            b.Description == command.Description &&
            b.UserId == command.UserId)), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
