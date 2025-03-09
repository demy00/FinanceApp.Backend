namespace FinanceApp.Application.DTOs;

public record LoginResponse(bool Success, string? Token, string? RefreshToken, IEnumerable<string>? Errors);