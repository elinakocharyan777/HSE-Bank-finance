namespace HSEBank.Logic.Application.Interfaces;

public interface IBalanceService
{
    void RecalculateAccountBalance(Guid accountId);
}