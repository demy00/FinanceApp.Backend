using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Categories.Queries;

public sealed class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);

        if (category is null)
        {
            throw new CategoryNotFoundException(request.Id);
        }

        return new CategoryResponse(category.Id, category.Name, category.Description);
    }
}
