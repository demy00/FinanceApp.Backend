using FinanceApp.Application.Abstractions;
using FinanceApp.Application.BillItems.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;
using Moq;

namespace FinanceApp.UnitTests.Application.BillItems;

public class CreateBillItemItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddBillItem_AndSaveChanges()
    {
        // Arrange
        var billItemRepoMock = new Mock<IBillItemRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var handler = new CreateBillItemCommandHandler(billItemRepoMock.Object, unitOfWorkMock.Object);

        var userId = Guid.NewGuid();
        var command = new CreateBillItemCommand(
            "TestName",
            "TestDescription",
            new Category("TestName", "TestDescription", userId),
            new Money(100, "USD"),
            new Quantity(1),
            userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        billItemRepoMock.Verify(r => r.Add(It.Is<BillItem>(b =>
            b.Name == command.Name &&
            b.Description == command.Description &&
            b.Category == command.Category &&
            b.Price == command.Price &&
            b.Quantity == command.Quantity &&
            b.UserId == command.UserId)), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
