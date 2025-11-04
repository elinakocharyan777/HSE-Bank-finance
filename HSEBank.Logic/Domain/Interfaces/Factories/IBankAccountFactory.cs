using HSEBank.Logic.Domain.Entities;

namespace HSEBank.Logic.Domain.Interfaces.Factories;

public interface IBankAccountFactory
{
    BankAccount Create(string name, decimal initialBalance = 0m);
}