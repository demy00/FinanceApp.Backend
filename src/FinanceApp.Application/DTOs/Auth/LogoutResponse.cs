namespace FinanceApp.Application.DTOs.Auth;

public record LogoutResponse(bool Success, IEnumerable<string>? Errors);
