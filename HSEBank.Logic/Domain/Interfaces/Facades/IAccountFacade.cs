using HSEBank.Logic.Domain.Entities;

namespace HSEBank.Logic.Domain.Interfaces.Facades;

public interface IAccountFacade
{
    Guid CreateAccount(string name, decimal initialBalance = 0m);
    void RenameAccount(Guid accountId, string newName);
    bool DeleteAccount(Guid accountId);
    BankAccount? GetById(Guid accountId);
    IReadOnlyCollection<BankAccount> GetAll();
}