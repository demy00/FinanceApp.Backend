namespace FinanceApp.Application.DTOs;

public record UpdatePeriodRequest(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate);
