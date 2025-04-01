namespace FinanceApp.Application.DTOs;

public record BillResponse(
    Guid Id,
    string Name,
    string Description,
    MoneyDto TotalPrice);
