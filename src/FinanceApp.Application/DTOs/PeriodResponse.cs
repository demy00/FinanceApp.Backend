namespace FinanceApp.Application.DTOs;

public record PeriodResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    List<MoneyDto> TotalSpent);
