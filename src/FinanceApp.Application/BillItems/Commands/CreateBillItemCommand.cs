using FinanceApp.Domain.Entities;
using FinanceApp.Domain.ValueObjects;
using MediatR;

namespace FinanceApp.Application.BillItems.Commands;

public record CreateBillItemCommand(
    string Name,
    string Description,
    Category Category,
    Money Price,
    Quantity Quantity,
    Guid UserId) : IRequest;
