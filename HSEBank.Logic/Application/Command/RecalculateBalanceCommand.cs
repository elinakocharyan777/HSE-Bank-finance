using HSEBank.Logic.Application.Interfaces;

namespace HSEBank.Logic.Application.Commands;

public class RecalculateBalanceCommand : ICommand<bool>
{
    private readonly IBalanceService _service;
    private readonly Guid _accountId;

    public RecalculateBalanceCommand(IBalanceService service, Guid accountId)
    {
        _service = service;
        _accountId = accountId;
    }

    public bool Execute()
    {
        _service.RecalculateAccountBalance(_accountId);
        return true;
    }
}