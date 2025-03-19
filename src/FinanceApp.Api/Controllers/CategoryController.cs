using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Categories.Commands;
using FinanceApp.Application.Categories.Queries;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Api.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public CategoryController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var command = new CreateCategoryCommand(request.Name, request.Description, userId);

        await _sender.Send(command, cancellationToken);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var query = new GetCategoriesQuery(userId, searchTerm, sortColumn, sortOrder, page, pageSize);

        var categories = await _sender.Send(query, cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        try
        {
            return Ok(await _sender.Send(new GetCategoryQuery(id, userId), cancellationToken));
        }
        catch (CategoryNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var command = new UpdateCategoryCommand(id, request.Name, request.Description, userId);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        await _sender.Send(new DeleteCategoryCommand(id, userId), cancellationToken);

        return NoContent();
    }
}