using MediatR;

namespace FinanceApp.Application.Categories.Commands;

public record DeleteCategoryCommand(Guid Id, Guid UserId) : IRequest;
