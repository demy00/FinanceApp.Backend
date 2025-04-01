namespace FinanceApp.Application.DTOs;

public record CreateBillItemRequest(
        string Name,
        string Description,
        CategoryDto Category,
        MoneyDto Price,
        QuantityDto Quantity);