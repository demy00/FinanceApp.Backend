using FinanceApp.Application.DTOs.Auth;

namespace FinanceApp.Application.Abstractions;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request);
    Task<LogoutResponse> LogoutAsync(LogoutRequest request);
    Task<AuthResult?> RefreshTokenAsync(RefreshRequest request);
    Task<AuthResult> GenerateTokensAsync(string username);
}