﻿using FinanceApp.Application.BillItems.Commands;
using FluentValidation.TestHelper;

namespace FinanceApp.UnitTests.Application.Validators.BillItems;

public class DeleteBillItemCommandValidatorTests
{
    private readonly DeleteBillItemCommandValidator _validator;

    public DeleteBillItemCommandValidatorTests()
    {
        _validator = new DeleteBillItemCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new DeleteBillItemCommand(
            Id: Guid.Empty,
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Bill item Id is required.");
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var command = new DeleteBillItemCommand(
            Id: Guid.NewGuid(),
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
        var command = new DeleteBillItemCommand(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid()
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}