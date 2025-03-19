using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Categories.Commands;
using FinanceApp.Domain.Entities;
using Moq;

namespace FinanceApp.UnitTests.Application.Categories;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddCategory_AndSaveChanges()
    {
        // Arrange
        var categoryRepoMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var handler = new CreateCategoryCommandHandler(categoryRepoMock.Object, unitOfWorkMock.Object);

        var userId = Guid.NewGuid();
        var command = new CreateCategoryCommand("TestName", "TestDescription", userId);
        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(command, cancellationToken);

        // Assert
        categoryRepoMock.Verify(r => r.Add(It.Is<Category>(p =>
            p.Name == command.Name &&
            p.Description == command.Description &&
            p.UserId == command.UserId)), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }
}