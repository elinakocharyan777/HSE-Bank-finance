using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Interfaces;
using HSEBank.Logic.Domain.Interfaces.Factories;
using HSEBank.Logic.Domain.Interfaces.Facades;

namespace HSEBank.Logic.Domain.Facades;

public class AccountFacade : IAccountFacade
{
    private readonly IRepository<BankAccount> _repo;
    private readonly IBankAccountFactory _factory;

    public AccountFacade(IRepository<BankAccount> repo, IBankAccountFactory factory)
    {
        _repo = repo;
        _factory = factory;
    }

    public Guid CreateAccount(string name, decimal initialBalance = 0m)
    {
        var acc = _factory.Create(name, initialBalance);
        _repo.Add(acc);
        return acc.Id;
    }

    public void RenameAccount(Guid accountId, string newName)
    {
        var acc = _repo.GetById(accountId) ?? throw new InvalidOperationException("Account not found");
        acc.Rename(newName);
        _repo.Update(acc);
    }

    public bool DeleteAccount(Guid accountId) => _repo.Remove(accountId);
    public BankAccount? GetById(Guid accountId) => _repo.GetById(accountId);
    public IReadOnlyCollection<BankAccount> GetAll() => _repo.GetAll();
}