namespace FinanceApp.Application.DTOs;

public record BillItemResponse(
    Guid Id,
    string Name,
    string Description,
    CategoryDto Category,
    MoneyDto Price,
    QuantityDto Quantity);
