using HSEBank.Logic.Application.Interfaces;
using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces;

namespace HSEBank.Logic.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly IRepository<Operation> _ops;
    private readonly IRepository<BankAccount> _accounts;

    public BalanceService(IRepository<Operation> ops, IRepository<BankAccount> accounts)
    {
        _ops = ops;
        _accounts = accounts;
    }

    public void RecalculateAccountBalance(Guid accountId)
    {
        var account = _accounts.GetById(accountId) ?? throw new InvalidOperationException("Account not found");

        var sum = _ops.GetAll()
            .Where(o => o.BankAccountId == accountId)
            .Sum(o => o.Type == FinanceType.Income ? o.Amount : -o.Amount);

        var diff = sum - account.Balance;
        if (diff > 0) account.ApplyIncome(diff);
        else if (diff < 0) account.ApplyExpense(-diff);

        _accounts.Update(account);
    }
}