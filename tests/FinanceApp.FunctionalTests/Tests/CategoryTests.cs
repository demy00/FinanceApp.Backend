using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.FunctionalTests.Setup;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace FinanceApp.FunctionalTests.Tests;

public class CategoryTests : TestBase
{
    private const string CategoriesBaseEndpoint = "/api/categories";
    public CategoryTests(PostgreSqlTestFixture fixture, WebApplicationFactory<Program> factory) : base(fixture, factory)
    {
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnOk()
    {
        await LoginAsync();

        var createCategoryRequest = new CreateCategoryRequest("New Description", "New Description");
        var createResponse = await Client.PostAsJsonAsync(CategoriesBaseEndpoint, createCategoryRequest);
        createResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetCategories_ShouldReturnCategories()
    {
        // Arrange
        var userId = await LoginAsync();
        await CreateTestCategory("Groceries", "Food and essentials");

        // Act
        var response = await Client.GetAsync($"{CategoriesBaseEndpoint}?page=1&pageSize=10");
        var categories = await response.Content.ReadFromJsonAsync<PageList<CategoryResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(categories);
        Assert.True(categories?.Items.Any(c => c.Name == "Groceries"));
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnCategory()
    {
        // Arrange
        var userId = await LoginAsync();
        var categoryId = await CreateTestCategory("Transport", "Travel expenses");

        // Act
        var response = await Client.GetAsync($"{CategoriesBaseEndpoint}/{categoryId}");
        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(category);
        Assert.Equal("Transport", category.Name);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnNotFound()
    {
        // Arrange
        var userId = await LoginAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{CategoriesBaseEndpoint}/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_ShouldReturnNoContent()
    {
        // Arrange
        var userId = await LoginAsync();
        var categoryId = await CreateTestCategory("Entertainment", "Movies and games");

        var updateRequest = new UpdateCategoryRequest("Updated Entertainment", "Updated Description");

        // Act
        var response = await Client.PostAsJsonAsync($"{CategoriesBaseEndpoint}/{categoryId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var updatedResponse = await Client.GetAsync($"{CategoriesBaseEndpoint}/{categoryId}");
        var updatedCategory = await updatedResponse.Content.ReadFromJsonAsync<CategoryResponse>();

        Assert.Equal("Updated Entertainment", updatedCategory?.Name);
        Assert.Equal("Updated Description", updatedCategory?.Description);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNoContent()
    {
        // Arrange
        var userId = await LoginAsync();
        var categoryId = await CreateTestCategory("Bills", "Monthly utility bills");

        // Act
        var response = await Client.DeleteAsync($"{CategoriesBaseEndpoint}/{categoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var deletedResponse = await Client.GetAsync($"{CategoriesBaseEndpoint}/{categoryId}");
        Assert.Equal(HttpStatusCode.NotFound, deletedResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteNonExistentCategory_ShouldReturnNotFound()
    {
        // Arrange
        var userId = await LoginAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"{CategoriesBaseEndpoint}/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Guid> CreateTestCategory(string name, string description)
    {
        var request = new CreateCategoryRequest(name, description);
        var response = await Client.PostAsJsonAsync($"{CategoriesBaseEndpoint}", request);
        response.EnsureSuccessStatusCode();

        var categoryResponse = await Client.GetAsync($"{CategoriesBaseEndpoint}?page=1&pageSize=10");
        var categories = await categoryResponse.Content.ReadFromJsonAsync<PageList<CategoryResponse>>();

        return categories?.Items.FirstOrDefault(c => c.Name == name)?.Id ?? throw new Exception("Category not found");
    }
}
