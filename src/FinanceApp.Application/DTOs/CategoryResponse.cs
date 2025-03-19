namespace FinanceApp.Application.DTOs;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description);