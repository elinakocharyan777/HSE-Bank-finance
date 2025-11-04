using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Interfaces.Factories;

namespace HSEBank.Logic.Domain.Factories;

public class BankAccountFactory : IBankAccountFactory
{
    public BankAccount Create(string name, decimal initialBalance = 0m) => new(name, initialBalance);
}