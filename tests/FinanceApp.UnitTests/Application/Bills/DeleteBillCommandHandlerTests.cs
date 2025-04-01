using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Bills.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Bills;

public class DeleteBillCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteBill_AndSaveChanges_WhenBillExists()
    {
        // Arrange
        var billRepoMock = new Mock<IBillRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var billId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var bill = new Bill("TestName", "TestDescription", userId);

        billRepoMock
            .Setup(r => r.GetDomainByIdAsync(billId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);

        var handler = new DeleteBillCommandHandler(billRepoMock.Object, unitOfWorkMock.Object);

        var command = new DeleteBillCommand(billId, userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        billRepoMock.Verify(r => r.Remove(bill), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBillNotFoundException_WhenBillDoesNotExist()
    {
        // Arrange
        var billRepoMock = new Mock<IBillRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var billId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        billRepoMock
            .Setup(r => r.GetDomainByIdAsync(billId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Bill);

        var handler = new DeleteBillCommandHandler(billRepoMock.Object, unitOfWorkMock.Object);

        var command = new DeleteBillCommand(billId, userId);

        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<BillNotFoundException>(() => handler.Handle(command, cancellationToken));
    }
}
