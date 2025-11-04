using HSEBank.Logic.Domain.Interfaces;

namespace HSEBank.Logic.Domain.Entities;

public class BankAccount : IEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public decimal Balance { get; private set; }

    public BankAccount(string name, decimal initialBalance = 0m)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required") : name;
        Balance = initialBalance;
    }

    public void Rename(string newName)
    {
        Name = string.IsNullOrWhiteSpace(newName) ? throw new ArgumentException("Name is required") : newName;
    }

    public void ApplyIncome(decimal amount) => Balance += amount;
    public void ApplyExpense(decimal amount) => Balance -= amount;
}