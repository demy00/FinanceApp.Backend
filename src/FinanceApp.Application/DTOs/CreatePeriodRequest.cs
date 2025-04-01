namespace FinanceApp.Application.DTOs;

public record CreatePeriodRequest(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate);
