using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;

namespace HSEBank.Logic.Domain.Interfaces.Facades;

public interface IOperationFacade
{
    Guid AddOperation(FinanceType type, Guid accountId, Guid categoryId, decimal amount, DateTime date, string? description = null);
    bool DeleteOperation(Guid operationId);
    bool UpdateOperation(Guid operationId, FinanceType type, Guid categoryId, decimal amount, DateTime date, string? description = null);
    IEnumerable<Operation> GetByAccount(Guid accountId);
    IEnumerable<Operation> GetByPeriod(DateTime from, DateTime to);
}