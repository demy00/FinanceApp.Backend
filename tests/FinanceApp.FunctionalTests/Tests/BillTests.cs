using FinanceApp.Application.DTOs;
using FinanceApp.Application.Helpers;
using FinanceApp.FunctionalTests.Setup;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace FinanceApp.FunctionalTests.Tests;

public class BillTests : TestBase
{

    public BillTests(PostgreSqlTestFixture fixture, WebApplicationFactory<Program> factory)
        : base(fixture, factory) { }

    [Fact]
    public async Task CreateBillItem_ShouldReturnOk()
    {
        // Arrange
        var userId = await LoginAsync();
        var request = new CreateBillItemRequest
        (
            Name: "Laptop",
            Description: "Gaming Laptop",
            Category: await CreateCategoryAsync("Electronics", ""),
            Price: new MoneyDto(1500.00m, "USD"),
            Quantity: new QuantityDto(1)
        );

        // Act
        var response = await Client.PostAsJsonAsync("/api/billItems", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBillItems_ShouldReturnBillItems()
    {
        // Arrange
        var userId = await LoginAsync();
        await CreateTestBillItem(
            "Book",
            "Programming book",
            await CreateCategoryAsync("Education", ""),
            new MoneyDto(30.00m, "USD"),
            new QuantityDto(1));

        // Act
        var response = await Client.GetAsync("/api/billItems?page=1&pageSize=10");
        var billItems = await response.Content.ReadFromJsonAsync<PageList<BillItemResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(billItems);
        Assert.True(billItems?.Items.Any(bi => bi.Name == "Book"));
    }

    [Fact]
    public async Task GetBillItemById_ShouldReturnBillItem()
    {
        // Arrange
        var userId = await LoginAsync();
        var billItemId = await CreateTestBillItem(
            "Headphones",
            "Wireless Headphones",
            await CreateCategoryAsync("Electronics", ""),
            new MoneyDto(200.00m, "USD"),
            new QuantityDto(1));
        // Act
        var response = await Client.GetAsync($"/api/billItems/{billItemId}");
        var billItem = await response.Content.ReadFromJsonAsync<BillItemResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(billItem);
        Assert.Equal("Headphones", billItem.Name);
    }

    [Fact]
    public async Task GetBillItemById_ShouldReturnNotFound()
    {
        // Arrange
        var userId = await LoginAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/billItems/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBillItem_ShouldReturnNoContent()
    {
        // Arrange
        var userId = await LoginAsync();
        var billItemId = await CreateTestBillItem(
            "Monitor",
            "4K Monitor",
            await CreateCategoryAsync("Electronics", ""),
            new MoneyDto(500.00m, "USD"),
            new QuantityDto(1));

        var updateRequest = new UpdateBillItemRequest(
            Name: "Updated Monitor",
            Description: "Updated 4K Monitor",
            Category: new CategoryDto("Updated Electronics", ""),
            new MoneyDto(450.00m, "USD"),
            new QuantityDto(2));

        // Act
        var response = await Client.PostAsJsonAsync($"/api/billItems/{billItemId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var updatedResponse = await Client.GetAsync($"/api/billItems/{billItemId}");
        var updatedBillItem = await updatedResponse.Content.ReadFromJsonAsync<BillItemResponse>();

        Assert.Equal("Updated Monitor", updatedBillItem?.Name);
        Assert.Equal("Updated 4K Monitor", updatedBillItem?.Description);
        Assert.Equal("Updated Electronics", updatedBillItem?.Category.Name);
        Assert.Equal(450.00m, updatedBillItem?.Price.Amount);
        Assert.Equal("USD", updatedBillItem?.Price.Currency);
        Assert.Equal(2, updatedBillItem?.Quantity.Value);
    }

    [Fact]
    public async Task DeleteBillItem_ShouldReturnNoContent()
    {
        // Arrange
        var userId = await LoginAsync();
        var billItemId = await CreateTestBillItem(
            "Tablet",
            "Android Tablet",
            await CreateCategoryAsync("Electronics", ""),
            new MoneyDto(300.00m, "USD"),
            new QuantityDto(1));

        // Act
        var response = await Client.DeleteAsync($"/api/billItems/{billItemId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var deletedResponse = await Client.GetAsync($"/api/billItems/{billItemId}");
        Assert.Equal(HttpStatusCode.NotFound, deletedResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteNonExistentBillItem_ShouldReturnNotFound()
    {
        // Arrange
        var userId = await LoginAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/billItems/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Guid> CreateTestBillItem(string name, string description, CategoryDto category, MoneyDto price, QuantityDto quantity)
    {
        var request = new CreateBillItemRequest
        (
            Name: name,
            Description: description,
            Category: category,
            Price: price,
            Quantity: quantity
        );

        var response = await Client.PostAsJsonAsync("/api/billItems", request);
        response.EnsureSuccessStatusCode();

        // Fetch bill items to get the ID of the newly created one
        var billItemResponse = await Client.GetAsync("/api/billItems?page=1&pageSize=10");
        var billItems = await billItemResponse.Content.ReadFromJsonAsync<PageList<BillItemResponse>>();

        return billItems?.Items.FirstOrDefault(c => c.Name == name)?.Id ?? throw new Exception("BillItem not found");
    }

    private async Task<CategoryDto> CreateCategoryAsync(string name, string description)
    {
        await Client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest(name, description));

        var categoryResponse = await Client.GetAsync("api/categories?page=1&pageSize=1");
        var categories = await categoryResponse.Content.ReadFromJsonAsync<PageList<CategoryResponse>>();
        var categoryDto = categories?.Items.FirstOrDefault();

        return new CategoryDto(categoryDto?.Name!, categoryDto?.Description!);
    }
}
