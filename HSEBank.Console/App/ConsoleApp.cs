using System;
using HSEBank.Console.App.Menu;
namespace HSEBank.Console.App;

public class ConsoleApp
{
    private readonly IServiceProvider _sp;
    private readonly ConsolePrompts _prompts;
    private readonly MenuActions _actions;

    public ConsoleApp(IServiceProvider sp)
    {
        _sp = sp;
        _prompts = new ConsolePrompts(sp);
        _actions = new MenuActions(sp, _prompts);
    }

    public void Run()
    {
        while (true)
        {
            System.Console.WriteLine("\nHSEBank Console");
            System.Console.WriteLine("1) Создать счёт");
            System.Console.WriteLine("2) Создать категорию");
            System.Console.WriteLine("3) Добавить операцию");
            System.Console.WriteLine("4) Импорт операций (CSV/JSON)");
            System.Console.WriteLine("5) Экспорт операций (CSV/JSON)");
            System.Console.WriteLine("6) Редактировать операцию");
            System.Console.WriteLine("7) Пересчитать баланс счёта");
            System.Console.WriteLine("8) Показать счета и баланс");
            System.Console.WriteLine("9) Аналитика за период");
            System.Console.WriteLine("0) Выход");
            System.Console.Write("Выбор: ");

            var choice = System.Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1": _actions.CreateAccount(); break;
                    case "2": _actions.CreateCategory(); break;
                    case "3": _actions.AddOperation(); break;
                    case "4": _actions.ImportOps(); break;
                    case "5": _actions.ExportOps(); break;
                    case "6": _actions.EditOperation(); break; 
                    case "7": _actions.RecalcBalance(); break;
                    case "8": _actions.ShowAccounts(); break;
                    case "9": _actions.ShowAnalytics(); break;
                    case "0": return;
                    default: System.Console.WriteLine("Выбирайте только 0-9"); break;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}