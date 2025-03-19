using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Bills.Commands;
using FinanceApp.Application.Bills.Queries;
using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Api.Controllers;

[Route("api/bills")]
[ApiController]
public class BillController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public BillController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBillRequest requset, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var command = new CreateBillCommand(requset.Name, requset.Description, userId);

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
        var query = new GetBillsQuery(userId, searchTerm, sortColumn, sortOrder, page, pageSize);

        var bills = await _sender.Send(query, cancellationToken);

        return Ok(bills);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        try
        {
            return Ok(await _sender.Send(new GetBillQuery(id, userId), cancellationToken));
        }
        catch (BillNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBillRequest requset, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var command = new UpdateBillCommand(id, requset.Name, requset.Description, userId);

        await _sender.Send(command, cancellationToken);

        return Ok();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Create(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        await _sender.Send(new DeleteBillCommand(id, userId), cancellationToken);

        return NoContent();
    }
}
