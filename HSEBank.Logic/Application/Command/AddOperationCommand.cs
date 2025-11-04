using HSEBank.Logic.Application.Interfaces;
using HSEBank.Logic.Domain.Enums;
using HSEBank.Logic.Domain.Interfaces.Facades;

namespace HSEBank.Logic.Application.Commands;

public class AddOperationCommand : ICommand<Guid>
{
    private readonly IOperationFacade _facade;
    private readonly FinanceType _type;
    private readonly Guid _accountId;
    private readonly Guid _categoryId;
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly string? _description;

    public AddOperationCommand(IOperationFacade facade, FinanceType type, Guid accountId, Guid categoryId, decimal amount, DateTime date, string? description = null)
    {
        _facade = facade;
        _type = type;
        _accountId = accountId;
        _categoryId = categoryId;
        _amount = amount;
        _date = date;
        _description = description;
    }

    public Guid Execute() =>
        _facade.AddOperation(_type, _accountId, _categoryId, _amount, _date, _description);
}