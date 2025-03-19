using MediatR;

namespace FinanceApp.Application.Categories.Commands;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description,
    Guid UserId) : IRequest;
