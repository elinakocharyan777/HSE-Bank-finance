namespace HSEBank.Logic.Application.Interfaces;

public interface ICommand<out TResult>
{
    TResult Execute();
}