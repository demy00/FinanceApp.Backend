using FinanceApp.Application.Abstractions;
using FinanceApp.Application.BillItems.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using FinanceApp.Domain.ValueObjects;
using Moq;

namespace FinanceApp.UnitTests.Application.BillItems;

public class UpdateBillItemCommandHandlerTests
{

    [Fact]
    public async Task Handle_ShouldUpdateBillItem_AndSaveChanges()
    {
        // Arrange
        var billItemRepoMock = new Mock<IBillItemRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var billItemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var category = new Category("TestName", "TestDescription", userId);
        var price = new Money(100, "USD");
        var quantity = new Quantity(1);

        var billItem = new BillItem("OldName", "OldDescription", category, price, quantity, userId);

        billItemRepoMock
            .Setup(r => r.GetDomainByIdAsync(billItemId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(billItem);

        var handler = new UpdateBillItemCommandHandler(billItemRepoMock.Object, unitOfWorkMock.Object);

        var newCategory = new Category("TestName", "TestDescription", userId);
        var newPrice = new Money(100, "USD");
        var newQuantity = new Quantity(1);
        var command = new UpdateBillItemCommand(
            billItemId,
            "NewName",
            "NewDescription",
            newCategory,
            newPrice,
            newQuantity,
            userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal("NewName", billItem.Name);
        Assert.Equal("NewDescription", billItem.Description);
        Assert.Equal(newCategory, billItem.Category);
        Assert.Equal(newPrice, billItem.Price);
        Assert.Equal(newQuantity, billItem.Quantity);

        billItemRepoMock.Verify(r => r.Update(billItem), Times.Once);

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

        var handler = new UpdateBillItemCommandHandler(billItemRepoMock.Object, unitOfWorkMock.Object);

        var command = new UpdateBillItemCommand(
            billItemId,
            "NewName",
            "NewDescription",
            new Category("TestName", "TestDescription", userId),
            new Money(100, "USD"),
            new Quantity(1),
            userId);

        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<BillItemNotFoundException>(() => handler.Handle(command, cancellationToken));
    }
}
