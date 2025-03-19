using FinanceApp.Application.BillItems.Commands;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace FinanceApp.UnitTests.Application.Validators.BillItems;

public class CreateBillItemCommandValidatorTests
{
    private readonly CreateBillItemCommandValidator _validator;

    public CreateBillItemCommandValidatorTests()
    {
        _validator = new CreateBillItemCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateBillItemCommand(
            Name: "",
            Description: "Some description",
            Category: new Category("Name", "Description", userId),
            Price: new Money(10m, "USD"),
            Quantity: new Quantity(1),
            UserId: userId
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Bill item name is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var longName = new string('a', 101);
        var command = new CreateBillItemCommand(
            Name: longName,
            Description: "Some description",
            Category: new Category("Name", "Description", userId),
            Price: new Money(10m, "USD"),
            Quantity: new Quantity(1),
            UserId: userId
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Bill item name must not exceed 100 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var longDescription = new string('a', 501);
        var command = new CreateBillItemCommand(
            Name: "Some name",
            Description: longDescription,
            Category: new Category("Name", "Description", userId),
            Price: new Money(10m, "USD"),
            Quantity: new Quantity(1),
            UserId: userId
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Zero()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateBillItemCommand(
            Name: "Some name",
            Description: "Some description",
            Category: new Category("Name", "Description", userId),
            Price: new Money(0m, "USD"),
            Quantity: new Quantity(1),
            UserId: userId
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price must be greater than zero.");
    }

    [Fact]
    public void Should_Have_Error_When_Currency_Is_Zero()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateBillItemCommand(
            Name: "Some name",
            Description: "Some description",
            Category: new Category("Name", "Description", userId),
            Price: new Money(10m, ""),
            Quantity: new Quantity(1),
            UserId: userId
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Currency must be provided.");
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateBillItemCommand(
            Name: "Some name",
            Description: "Some description",
            Category: new Category("Name", "Description", userId),
            Price: new Money(10m, "USD"),
            Quantity: new Quantity(1),
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
        var userId = Guid.NewGuid();
        var command = new CreateBillItemCommand(
            Name: "Some name",
            Description: "Some description",
            Category: new Category("Name", "Description", userId),
            Price: new Money(10m, "USD"),
            Quantity: new Quantity(1),
            UserId: userId
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
