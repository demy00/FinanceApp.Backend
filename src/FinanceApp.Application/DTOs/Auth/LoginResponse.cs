namespace FinanceApp.Application.DTOs.Auth;

public record LoginResponse(bool Success, string? Token, string? RefreshToken, IEnumerable<string>? Errors);