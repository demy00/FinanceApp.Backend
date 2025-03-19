using FinanceApp.Application.Periods.Commands;
using FluentValidation.TestHelper;

namespace FinanceApp.UnitTests.Application.Validators.Periods;

public class CreatePeriodCommandValidatorTests
{
    private readonly CreatePeriodCommandValidator _validator;

    public CreatePeriodCommandValidatorTests()
    {
        _validator = new CreatePeriodCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var command = new CreatePeriodCommand(
            Name: "",
            Description: "Some description",
            StartDate: DateTime.Now,
            EndDate: DateTime.Now.AddDays(1),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Period name is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new CreatePeriodCommand(
            Name: longName,
            Description: "Some description",
            StartDate: DateTime.Now,
            EndDate: DateTime.Now.AddDays(1),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Period name must not exceed 100 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var longDescription = new string('a', 501);
        var command = new CreatePeriodCommand(
            Name: "Valid Name",
            Description: longDescription,
            StartDate: DateTime.Now,
            EndDate: DateTime.Now.AddDays(1),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_StartDate_Is_Invalid()
    {
        // Arrange
        var command = new CreatePeriodCommand(
            Name: "Valid Name",
            Description: "Valid description",
            StartDate: DateTime.MinValue,
            EndDate: DateTime.Now.AddDays(1),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("StartDate must be a valid date.");
    }

    [Fact]
    public void Should_Have_Error_When_EndDate_Is_Invalid()
    {
        // Arrange
        var command = new CreatePeriodCommand(
            Name: "Valid Name",
            Description: "Valid description",
            StartDate: DateTime.Now,
            EndDate: DateTime.MinValue,
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("EndDate must be a valid date.");
    }

    [Fact]
    public void Should_Have_Error_When_EndDate_Is_Before_StartDate()
    {
        // Arrange
        var startDate = DateTime.Now;
        var command = new CreatePeriodCommand(
            Name: "Valid Name",
            Description: "Valid description",
            StartDate: startDate,
            EndDate: startDate.AddDays(-1),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("EndDate must be after StartDate.");
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var command = new CreatePeriodCommand(
            Name: "Valid Name",
            Description: "Valid description",
            StartDate: DateTime.Now,
            EndDate: DateTime.Now.AddDays(1),
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
        var command = new CreatePeriodCommand(
            Name: "Valid Name",
            Description: "Valid description",
            StartDate: DateTime.Now,
            EndDate: DateTime.Now.AddDays(1),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
