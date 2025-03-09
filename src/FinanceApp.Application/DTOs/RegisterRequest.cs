namespace FinanceApp.Application.DTOs;

public record RegisterRequest(string Username, string Email, string Password, string FullName);