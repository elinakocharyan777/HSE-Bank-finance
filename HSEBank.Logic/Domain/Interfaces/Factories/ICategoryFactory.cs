using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Enums;

namespace HSEBank.Logic.Domain.Interfaces.Factories;

public interface ICategoryFactory
{
    Category Create(string name, FinanceType type);
}