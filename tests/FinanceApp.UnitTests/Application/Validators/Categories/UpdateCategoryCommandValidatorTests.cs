﻿using FinanceApp.Application.Categories.Commands;
using FluentValidation.TestHelper;

namespace FinanceApp.UnitTests.Application.Validators.Categories;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator;

    public UpdateCategoryCommandValidatorTests()
    {
        _validator = new UpdateCategoryCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new UpdateCategoryCommand(
            Id: Guid.Empty,
            Name: "Some name",
            Description: "Some description",
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Category Id is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var command = new UpdateCategoryCommand(
            Id: Guid.NewGuid(),
            Name: "",
            Description: "Some description",
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Category name is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new UpdateCategoryCommand(
            Id: Guid.NewGuid(),
            Name: longName,
            Description: "Some description",
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Category name must not exceed 100 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var longDescription = new string('a', 501);
        var command = new UpdateCategoryCommand(
            Id: Guid.NewGuid(),
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
        var command = new UpdateCategoryCommand(
            Id: Guid.NewGuid(),
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
        var command = new UpdateCategoryCommand(
            Id: Guid.NewGuid(),
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
