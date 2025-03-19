using FinanceApp.Application.Abstractions;
using FinanceApp.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authenticationService.RegisterUserAsync(request);

        if (!response.Success)
            return BadRequest(response.Errors);

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authenticationService.LoginAsync(request);

        if (!response.Success)
            return BadRequest(response.Errors);

        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var response = await _authenticationService.LogoutAsync(request);

        if (!response.Success)
            return BadRequest(response.Errors);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
    {
        var authResult = await _authenticationService.RefreshTokenAsync(request);

        if (authResult == null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token." });
        }

        return Ok(authResult);
    }
}
