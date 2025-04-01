using FinanceApp.Application.Bills.Queries;
using FluentValidation.TestHelper;

namespace FinanceApp.UnitTests.Application.Validators.Bills;

public class GetBillQueryValidatorTests
{
    private readonly GetBillQueryValidator _validator;

    public GetBillQueryValidatorTests()
    {
        _validator = new GetBillQueryValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var query = new GetBillQuery(
            Id: Guid.Empty,
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Bill Id is required.");
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var query = new GetBillQuery(
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
        var query = new GetBillQuery(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}