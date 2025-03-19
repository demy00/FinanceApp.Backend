using FinanceApp.Application.Abstractions;
using FinanceApp.Application.BillItems.Commands;
using FinanceApp.Application.BillItems.Queries;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using FinanceApp.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Api.Controllers;

[Route("api/billItems")]
[ApiController]
public class BillItemController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public BillItemController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBillItemRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var command = new CreateBillItemCommand(
            request.Name,
            request.Description,
            new Category(request.Category.Name, request.Category.Description, userId),
            new Money(request.Price.Amount, request.Price.Currency),
            new Quantity(request.Quantity.Value),
            userId);

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
        var query = new GetBillItemsQuery(userId, searchTerm, sortColumn, sortOrder, page, pageSize);

        var categories = await _sender.Send(query, cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        try
        {
            return Ok(await _sender.Send(new GetBillItemQuery(id, userId), cancellationToken));
        }
        catch (CategoryNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBillItemRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var command = new UpdateBillItemCommand(
            id,
            request.Name,
            request.Description,
            new Category(request.Category.Name, request.Category.Description, userId),
            new Money(request.Price.Amount, request.Price.Currency),
            new Quantity(request.Quantity.Value),
            userId);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        await _sender.Send(new DeleteBillItemCommand(id, userId), cancellationToken);

        return NoContent();
    }
}
