using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Bills.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Bills;

public class UpdateBillCommandHandlerTests
{

    [Fact]
    public async Task Handle_ShouldUpdateBill_AndSaveChanges()
    {
        // Arrange
        var billRepoMock = new Mock<IBillRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var billId = Guid.NewGuid();
        var oldStartDate = DateTime.UtcNow;
        var oldEndDate = oldStartDate.AddDays(10);
        var userId = Guid.NewGuid();

        var bill = new Bill("OldName", "OldDescription", userId);

        billRepoMock
            .Setup(r => r.GetDomainByIdAsync(billId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bill);

        var handler = new UpdateBillCommandHandler(billRepoMock.Object, unitOfWorkMock.Object);

        var newStartDate = DateTime.UtcNow.AddDays(1);
        var newEndDate = newStartDate.AddDays(10);
        var command = new UpdateBillCommand(
            billId,
            "NewName",
            "NewDescription",
            userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal("NewName", bill.Name);
        Assert.Equal("NewDescription", bill.Description);

        billRepoMock.Verify(r => r.Update(bill), Times.Once);

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

        var handler = new UpdateBillCommandHandler(billRepoMock.Object, unitOfWorkMock.Object);

        var command = new UpdateBillCommand(
            billId,
            "NewName",
            "NewDescription",
            userId);

        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<BillNotFoundException>(() => handler.Handle(command, cancellationToken));
    }
}
