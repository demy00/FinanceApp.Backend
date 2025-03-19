using FinanceApp.Application.Abstractions;
using FinanceApp.Application.BillItems.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using FinanceApp.Domain.ValueObjects;
using Moq;

namespace FinanceApp.UnitTests.Application.BillItems;

public class DeleteBillItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteBillItem_AndSaveChangesWhenBillItemExists()
    {
        // Arrange
        var billItemRepoMock = new Mock<IBillItemRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var billItemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var billItem = new BillItem(
            "TestName",
            "TestDescription",
            new Category("TestName", "TestDescription", userId),
            new Money(100, "USD"),
            new Quantity(1),
            userId);

        billItemRepoMock
            .Setup(r => r.GetDomainByIdAsync(billItemId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(billItem);

        var handler = new DeleteBillItemCommandHandler(billItemRepoMock.Object, unitOfWorkMock.Object);

        var command = new DeleteBillItemCommand(billItemId, userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        billItemRepoMock.Verify(r => r.Remove(billItem), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBillItemNotFoundException_WhenBillItemDoesNotExist()
    {
        // Arrange
        var billItemRepoMock = new Mock<IBillItemRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var billItemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        billItemRepoMock
            .Setup(r => r.GetDomainByIdAsync(billItemId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as BillItem);

        var handler = new DeleteBillItemCommandHandler(billItemRepoMock.Object, unitOfWorkMock.Object);

        var command = new DeleteBillItemCommand(billItemId, userId);

        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<BillItemNotFoundException>(() => handler.Handle(command, cancellationToken));
    }
}
