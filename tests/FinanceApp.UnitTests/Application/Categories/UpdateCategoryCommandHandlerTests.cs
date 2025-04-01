using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Categories.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Categories;

public class UpdateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdateCategory_AndSaveChanges()
    {
        // Arrange
        var categoryRepoMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var categoryId = Guid.NewGuid();
        var oldStartDate = DateTime.UtcNow;
        var oldEndDate = oldStartDate.AddDays(10);
        var userId = Guid.NewGuid();

        var category = new Category("OldName", "OldDescription", userId);

        categoryRepoMock
            .Setup(r => r.GetDomainByIdAsync(categoryId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var handler = new UpdateCategoryCommandHandler(categoryRepoMock.Object, unitOfWorkMock.Object);

        var command = new UpdateCategoryCommand(
            categoryId,
            "NewName",
            "NewDescription",
            userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal("NewName", category.Name);
        Assert.Equal("NewDescription", category.Description);

        categoryRepoMock.Verify(r => r.Update(category), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowCategoryNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryRepoMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        categoryRepoMock
            .Setup(r => r.GetDomainByIdAsync(categoryId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Category);

        var handler = new UpdateCategoryCommandHandler(categoryRepoMock.Object, unitOfWorkMock.Object);

        var command = new UpdateCategoryCommand(
            categoryId,
            "NewName",
            "NewDescription",
            userId);

        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(() => handler.Handle(command, cancellationToken));
    }
}
