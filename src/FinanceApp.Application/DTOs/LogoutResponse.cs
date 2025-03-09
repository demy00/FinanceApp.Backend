namespace FinanceApp.Application.DTOs;

public record LogoutResponse(bool Success, IEnumerable<string>? Errors);
