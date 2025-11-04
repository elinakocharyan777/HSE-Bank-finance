using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;

namespace HSEBank.Logic.Domain.Interfaces.Factories;

public interface IOperationFactory
{
    Operation Create(FinanceType type, Guid bankAccountId, Guid categoryId, decimal amount, DateTime date, string? description = null);
}