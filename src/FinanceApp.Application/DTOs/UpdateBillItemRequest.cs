namespace FinanceApp.Application.DTOs;

public record UpdateBillItemRequest(
    string Name,
    string Description,
    CategoryDto Category,
    MoneyDto Price,
    QuantityDto Quantity);
