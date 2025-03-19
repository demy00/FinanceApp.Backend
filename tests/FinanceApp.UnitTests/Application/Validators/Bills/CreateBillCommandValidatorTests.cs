using FinanceApp.Application.Bills.Commands;
using FluentValidation.TestHelper;

namespace FinanceApp.UnitTests.Application.Validators.Bills;

public class CreateBillCommandValidatorTests
{
    private readonly CreateBillCommandValidator _validator;

    public CreateBillCommandValidatorTests()
    {
        _validator = new CreateBillCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var command = new CreateBillCommand(
            Name: "",
            Description: "Some description",
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Bill name is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new CreateBillCommand(
            Name: longName,
            Description: "Some description",
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Bill name must not exceed 100 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var longDescription = new string('a', 501);
        var command = new CreateBillCommand(
            Name: "Some name",
            Description: longDescription,
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var command = new CreateBillCommand(
            Name: "Some name",
            Description: "Some description",
            UserId: Guid.Empty
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        // Arrange
        var command = new CreateBillCommand(
            Name: "Some name",
            Description: "Some description",
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
