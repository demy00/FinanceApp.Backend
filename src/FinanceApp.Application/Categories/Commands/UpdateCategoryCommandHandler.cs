using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Categories.Commands;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetDomainByIdAsync(request.Id, request.UserId, cancellationToken);

        if (category is null)
        {
            throw new CategoryNotFoundException(request.Id);
        }

        category.Update(request.Name, request.Description);

        _categoryRepository.Update(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
