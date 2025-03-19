using FinanceApp.Application.DTOs.Auth;
using FinanceApp.FunctionalTests.Helpers;
using FinanceApp.FunctionalTests.Setup;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinanceApp.FunctionalTests;

[Collection("Sequential Tests Collection")]
public abstract class TestBase : IAsyncLifetime, IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly PostgreSqlTestFixture Fixture;
    protected readonly HttpClient Client;

    public TestBase(PostgreSqlTestFixture fixture, WebApplicationFactory<Program> factory)
    {
        Fixture = fixture;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await Fixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public async Task<Guid> LoginAsync()
    {
        var userHelper = new UserHelper();
        var registerRequest = new RegisterRequest(userHelper.UserName, userHelper.Email, userHelper.Password, userHelper.FullName);
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest(userHelper.UserName, userHelper.Password);
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            throw new Exception("Token not returned.");

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        return ExtractUserIdFromToken(loginResult.Token);
    }

    private Guid ExtractUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null)
            throw new Exception("User ID not found in token.");

        return Guid.Parse(userIdClaim.Value);
    }
}
