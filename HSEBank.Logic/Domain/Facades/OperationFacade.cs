using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces;
using HSEBank.Logic.Domain.Interfaces.Facades;
using HSEBank.Logic.Domain.Interfaces.Factories;

namespace HSEBank.Logic.Domain.Facades;

public class OperationFacade : IOperationFacade
{
    private readonly IRepository<Operation> _operations;
    private readonly IRepository<BankAccount> _accounts;
    private readonly IOperationFactory _factory;

    public OperationFacade(IRepository<Operation> operations, IRepository<BankAccount> accounts, IOperationFactory factory)
    {
        _operations = operations;
        _accounts = accounts;
        _factory = factory;
    }

    public Guid AddOperation(FinanceType type, Guid accountId, Guid categoryId, decimal amount, DateTime date, string? description = null)
    {
        var account = _accounts.GetById(accountId) ?? throw new InvalidOperationException("Account not found");

        var op = _factory.Create(type, accountId, categoryId, amount, date, description);
        _operations.Add(op);

        if (type == FinanceType.Income) account.ApplyIncome(amount);
        else account.ApplyExpense(amount);
        _accounts.Update(account);

        return op.Id;
    }

    public bool DeleteOperation(Guid operationId)
    {
        var op = _operations.GetById(operationId);
        if (op is null) return false;

        var account = _accounts.GetById(op.BankAccountId);
        if (account != null)
        {
            // обратная корректировка баланса
            if (op.Type == FinanceType.Income) account.ApplyExpense(op.Amount);
            else account.ApplyIncome(op.Amount);
            _accounts.Update(account);
        }

        return _operations.Remove(operationId);
    }
    public bool UpdateOperation(Guid operationId, FinanceType type, Guid categoryId, decimal amount, DateTime date, string? description = null)
    {
        var op = _operations.GetById(operationId);
        if (op is null) return false;

        var account = _accounts.GetById(op.BankAccountId) ?? throw new InvalidOperationException("Account not found");

        // Считаем изменение влияния на баланс
        var currentEffect = op.Type == FinanceType.Income ? op.Amount : -op.Amount;
        var newEffect = type == FinanceType.Income ? amount : -amount;
        var delta = newEffect - currentEffect;

        // Применяем дельту к балансу
        if (delta > 0) account.ApplyIncome(delta);
        else if (delta < 0) account.ApplyExpense(-delta);

        // Обновляем саму операцию
        op.Edit(type, categoryId, amount, date, description);
        _operations.Update(op);
        _accounts.Update(account);

        return true;
    }
    public IEnumerable<Operation> GetByAccount(Guid accountId) =>
        _operations.GetAll().Where(o => o.BankAccountId == accountId).OrderBy(o => o.Date);

    public IEnumerable<Operation> GetByPeriod(DateTime from, DateTime to) =>
        _operations.GetAll().Where(o => o.Date >= from && o.Date <= to).OrderBy(o => o.Date);
}