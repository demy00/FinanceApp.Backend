using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs.Auth;
using FinanceApp.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinanceApp.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthenticationService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
            return new RegisterResponse(false, ["Username is already taken."]);

        existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return new RegisterResponse(false, [$"{request.Email} is already registered."]);

        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return new RegisterResponse(false, result.Errors.Select(e => e.Description));

        return new RegisterResponse(true, null);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return new LoginResponse(false, null, null, ["Invalid username or password."]);
        }

        var authResult = await GenerateTokensAsync(user.UserName!);
        return new LoginResponse(true, authResult.Token, authResult.RefreshToken, null);
    }

    public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (token == null) return new LogoutResponse(false, ["Invalid refresh token"]);

        token.IsRevoked = true;
        await _context.SaveChangesAsync();
        return new LogoutResponse(true, null);
    }

    public async Task<AuthResult?> RefreshTokenAsync(RefreshRequest request)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow);

        if (token?.User == null) return null;

        token.IsRevoked = true;
        var newRefreshToken = await GenerateRefreshTokenAsync(token.User);
        await _context.SaveChangesAsync();

        var newJwtToken = GenerateJwtToken(token.User);
        return new AuthResult(newJwtToken, newRefreshToken);
    }

    public async Task<AuthResult> GenerateTokensAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return null!;

        var jwtToken = GenerateJwtToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return new AuthResult(jwtToken, refreshToken);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        var secretKey = _configuration["JwtSettings:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT Secret Key is not configured.");
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken.Token;
    }
}