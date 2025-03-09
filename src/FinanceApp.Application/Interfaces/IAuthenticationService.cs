using FinanceApp.Application.DTOs;

namespace FinanceApp.Application.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request);
    Task<LogoutResponse> LogoutAsync(LogoutRequest request);
    Task<AuthResult?> RefreshTokenAsync(RefreshRequest request);
    Task<AuthResult> GenerateTokensAsync(string username);
}