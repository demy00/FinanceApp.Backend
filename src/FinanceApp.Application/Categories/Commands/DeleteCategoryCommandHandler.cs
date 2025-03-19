using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Exceptions;
using MediatR;

namespace FinanceApp.Application.Categories.Commands;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetDomainByIdAsync(request.Id, request.UserId, cancellationToken);

        if (category is null)
        {
            throw new CategoryNotFoundException(request.Id);
        }

        _categoryRepository.Remove(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}