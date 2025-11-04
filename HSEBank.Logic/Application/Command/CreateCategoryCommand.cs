using HSEBank.Logic.Application.Interfaces;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces.Facades;

namespace HSEBank.Logic.Application.Commands;

public class CreateCategoryCommand : ICommand<Guid>
{
    private readonly ICategoryFacade _facade;
    private readonly string _name;
    private readonly FinanceType _type;

    public CreateCategoryCommand(ICategoryFacade facade, string name, FinanceType type)
    {
        _facade = facade;
        _name = name;
        _type = type;
    }

    public Guid Execute() => _facade.CreateCategory(_name, _type);
}