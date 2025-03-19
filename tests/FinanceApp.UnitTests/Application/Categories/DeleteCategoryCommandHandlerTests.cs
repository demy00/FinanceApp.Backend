using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Categories.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Moq;

namespace FinanceApp.UnitTests.Application.Categories;

public class DeleteCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteCategory_AndSaveChangesWhenCategoryExists()
    {
        // Arrange
        var categoryRepoMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var category = new Category("TestName", "TestDescription", userId);

        categoryRepoMock
            .Setup(r => r.GetDomainByIdAsync(categoryId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var handler = new DeleteCategoryCommandHandler(categoryRepoMock.Object, unitOfWorkMock.Object);

        var command = new DeleteCategoryCommand(categoryId, userId);

        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        categoryRepoMock.Verify(r => r.Remove(category), Times.Once);
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

        var handler = new DeleteCategoryCommandHandler(categoryRepoMock.Object, unitOfWorkMock.Object);

        var command = new DeleteCategoryCommand(categoryId, userId);

        var cancellationToken = new CancellationToken();

        // Act & Assert
        await Assert.ThrowsAsync<CategoryNotFoundException>(() => handler.Handle(command, cancellationToken));
    }
}
