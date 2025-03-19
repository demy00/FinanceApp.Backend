using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs;
using FinanceApp.Application.Periods.Commands;
using FinanceApp.Application.Periods.Queries;
using FinanceApp.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Api.Controllers;

[Route("api/periods")]
[ApiController]
public class PeriodController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public PeriodController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePeriodRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var command = new CreatePeriodCommand(
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate,
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
        var query = new GetPeriodsQuery(userId, searchTerm, sortColumn, sortOrder, page, pageSize);

        var periods = await _sender.Send(query, cancellationToken);

        return Ok(periods);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        try
        {
            return Ok(await _sender.Send(new GetPeriodQuery(id, userId), cancellationToken));
        }
        catch (PeriodNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePeriodRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var command = new UpdatePeriodCommand(
            id,
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate,
            userId);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        await _sender.Send(new DeletePeriodCommand(id, userId), cancellationToken);

        return NoContent();
    }
}
