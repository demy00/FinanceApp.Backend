using FinanceApp.Application.BillItems.Queries;
using FluentValidation.TestHelper;

namespace FinanceApp.UnitTests.Application.Validators.BillItems;

public class GetBillItemsQueryValidatorTests
{
    private readonly GetBillItemsQueryValidator _validator;

    public GetBillItemsQueryValidatorTests()
    {
        _validator = new GetBillItemsQueryValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var query = new GetBillItemsQuery(
            UserId: Guid.Empty,
            SearchTerm: "",
            SortColumn: "",
            SortOrder: "asc",
            Page: 1,
            PageSize: 10
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Page_Is_Zero()
    {
        // Arrange
        var query = new GetBillItemsQuery(
            UserId: Guid.NewGuid(),
            SearchTerm: "",
            SortColumn: "",
            SortOrder: "asc",
            Page: 0,
            PageSize: 10
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("Page number must be greater than zero.");
    }

    [Fact]
    public void Should_Have_Error_When_PageSize_Is_Zero()
    {
        // Arrange
        var query = new GetBillItemsQuery(
            UserId: Guid.NewGuid(),
            SearchTerm: "",
            SortColumn: "",
            SortOrder: "asc",
            Page: 1,
            PageSize: 0
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be greater than zero.");
    }

    [Fact]
    public void Should_Have_Error_When_PageSize_Is_Greater_Than_100()
    {
        // Arrange
        var query = new GetBillItemsQuery(
            UserId: Guid.NewGuid(),
            SearchTerm: "",
            SortColumn: "",
            SortOrder: "asc",
            Page: 1,
            PageSize: 101
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must not exceed 100.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("notascordesc")]
    public void Should_Have_Error_When_SortOrder_Is_Greater_Than_100(string sortOrder)
    {
        // Arrange
        var query = new GetBillItemsQuery(
            UserId: Guid.NewGuid(),
            SearchTerm: "",
            SortColumn: "",
            SortOrder: sortOrder,
            Page: 1,
            PageSize: 10
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortOrder)
            .WithErrorMessage("SortOrder must be either 'asc' or 'desc'.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        // Arrange
        var query = new GetBillItemsQuery(
            UserId: Guid.NewGuid(),
            SearchTerm: "",
            SortColumn: "",
            SortOrder: "asc",
            Page: 1,
            PageSize: 10
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}