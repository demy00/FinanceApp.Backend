using FinanceApp.Application.Categories.Queries;
using FluentValidation.TestHelper;

namespace FinanceApp.UnitTests.Application.Validators.Categories;

public class GetCategoryQueryValidatorTests
{
    private readonly GetCategoryQueryValidator _validator;

    public GetCategoryQueryValidatorTests()
    {
        _validator = new GetCategoryQueryValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var query = new GetCategoryQuery(
            Id: Guid.Empty,
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Category Id is required.");
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var query = new GetCategoryQuery(
            Id: Guid.NewGuid(),
            UserId: Guid.Empty
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        // Arrange
        var query = new GetCategoryQuery(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}