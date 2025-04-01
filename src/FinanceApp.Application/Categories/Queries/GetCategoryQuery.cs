using FinanceApp.Application.DTOs;
using MediatR;

namespace FinanceApp.Application.Categories.Queries;

public record GetCategoryQuery(Guid Id, Guid UserId) : IRequest<CategoryResponse>;
