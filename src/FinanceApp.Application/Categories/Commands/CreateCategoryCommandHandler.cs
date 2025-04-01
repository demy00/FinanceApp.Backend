using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Entities;
using MediatR;

namespace FinanceApp.Application.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name, request.Description, request.UserId);

        _categoryRepository.Add(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
