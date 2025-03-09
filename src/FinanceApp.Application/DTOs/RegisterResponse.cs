namespace FinanceApp.Application.DTOs;

public record RegisterResponse(bool Success, IEnumerable<string>? Errors);
