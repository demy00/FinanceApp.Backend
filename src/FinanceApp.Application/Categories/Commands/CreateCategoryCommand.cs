using MediatR;

namespace FinanceApp.Application.Categories.Commands;

public record CreateCategoryCommand(
    string Name,
    string Description,
    Guid UserId) : IRequest;

