using FinanceApp.Application.DTOs.Auth;
using FinanceApp.FunctionalTests.Helpers;
using FinanceApp.FunctionalTests.Setup;
using System.Net.Http.Json;

namespace FinanceApp.FunctionalTests.Tests;

[Collection("Auth Collection")]
public class AuthTests : IAsyncLifetime, IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly PostgreSqlTestFixture _fixture;
    private readonly HttpClient _client;

    public AuthTests(PostgreSqlTestFixture fixture, CustomWebApplicationFactory<Program> factory)
    {
        _fixture = fixture;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_Should_Return_Success_Message()
    {
        // Arrange
        var userHelper = new UserHelper();
        var registerRequest = new RegisterRequest(userHelper.UserName, userHelper.Email, userHelper.Password, userHelper.FullName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var resultString = await response.Content.ReadAsStringAsync();
        Assert.Contains("User registered successfully", resultString);
    }

    [Fact]
    public async Task Login_Should_Return_Authentication_Response()
    {
        // Arrange
        var userHelper = new UserHelper();
        var registerRequest = new RegisterRequest(userHelper.UserName, userHelper.Email, userHelper.Password, userHelper.FullName);
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        // Act
        var loginRequest = new LoginRequest(userHelper.UserName, userHelper.Password);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResult);
        Assert.True(loginResult.Success, "Login should succeed.");
        Assert.False(string.IsNullOrEmpty(loginResult.Token));
    }

    [Fact]
    public async Task Logout_Should_Return_Success_Response()
    {
        // Arrange
        var userHelper = new UserHelper();
        var registerRequest = new RegisterRequest(userHelper.UserName, userHelper.Email, userHelper.Password, userHelper.FullName);
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest(userHelper.UserName, userHelper.Password);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        var logoutRequest = new LogoutRequest(loginResult?.RefreshToken!);

        // Act
        var logoutResponse = await _client.PostAsJsonAsync("/api/auth/logout", logoutRequest);

        // Assert
        logoutResponse.EnsureSuccessStatusCode();
        var logoutResult = await logoutResponse.Content.ReadFromJsonAsync<LogoutResponse>();
        Assert.NotNull(logoutResult);
        Assert.True(logoutResult.Success);
    }

    [Fact]
    public async Task RefreshToken_Should_Return_New_Token()
    {
        // Arrange
        var userHelper = new UserHelper();
        var registerRequest = new RegisterRequest(userHelper.UserName, userHelper.Email, userHelper.Password, userHelper.FullName);
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest(userHelper.UserName, userHelper.Password);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        var refreshRequest = new RefreshRequest(loginResult?.RefreshToken!);

        // Act
        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        refreshResponse.EnsureSuccessStatusCode();
        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(refreshResult);
        Assert.False(string.IsNullOrEmpty(refreshResult.Token));
    }
}