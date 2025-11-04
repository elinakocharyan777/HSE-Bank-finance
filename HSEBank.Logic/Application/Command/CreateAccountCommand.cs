using HSEBank.Logic.Application.Interfaces;
using HSEBank.Logic.Domain.Interfaces.Facades;

namespace HSEBank.Logic.Application.Commands;

public class CreateAccountCommand : ICommand<Guid>
{
    private readonly IAccountFacade _facade;
    private readonly string _name;
    private readonly decimal _initial;

    public CreateAccountCommand(IAccountFacade facade, string name, decimal initialBalance = 0m)
    {
        _facade = facade;
        _name = name;
        _initial = initialBalance;
    }

    public Guid Execute() => _facade.CreateAccount(_name, _initial);
}