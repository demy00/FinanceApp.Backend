using FinanceApp.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace FinanceApp.Api;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation(ex, "Validation exception occurred");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            var result = JsonSerializer.Serialize(new { errors });
            await context.Response.WriteAsync(result);
        }
        catch (FinanceAppDomainException ex)
        {
            _logger.LogWarning(ex, "Business exception occurred");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { errors = ex.Message });
            await context.Response.WriteAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(ex.Message);
        }
    }
}