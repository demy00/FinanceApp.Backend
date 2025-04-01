namespace FinanceApp.Application.DTOs.Auth;

public record RegisterResponse(bool Success, IEnumerable<string>? Errors);
