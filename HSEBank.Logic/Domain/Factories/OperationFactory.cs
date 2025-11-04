using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces.Factories;

namespace HSEBank.Logic.Domain.Factories;

public class OperationFactory : IOperationFactory
{
    public Operation Create(FinanceType type, Guid bankAccountId, Guid categoryId, decimal amount, DateTime date, string? description = null)
        => new(type, bankAccountId, categoryId, amount, date, description);
}