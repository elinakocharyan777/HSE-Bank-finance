using System.Globalization;
using HSEBank.Console.App;
using HSEBank.Infrastructure.Repositories;
using HSEBank.Logic.Application.Analytics;
using HSEBank.Logic.Application.Exporting;
using HSEBank.Logic.Application.Importing;
using HSEBank.Logic.Application.Interfaces;
using HSEBank.Logic.Application.Services;
using HSEBank.Logic.Domain.Entities;
using HSEBank.Logic.Domain.Factories;
using HSEBank.Logic.Domain.Facades;
using HSEBank.Logic.Domain.Interfaces;
using HSEBank.Logic.Domain.Interfaces.Facades;
using HSEBank.Logic.Domain.Interfaces.Factories;
using HSEBank.Logic.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

// для чисел и дат
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// DI
var services = new ServiceCollection();

// Repositories (Proxy поверх InMemory)
services.AddSingleton<InMemoryRepository<BankAccount>>(); // внутреннее хранилище
services.AddSingleton<InMemoryRepository<Category>>();
services.AddSingleton<InMemoryRepository<Operation>>();

// Интерфейс IRepository<T> отдаём как кэширующий прокси
services.AddSingleton<IRepository<BankAccount>>(sp =>
    new CachingRepositoryProxy<BankAccount>(sp.GetRequiredService<InMemoryRepository<BankAccount>>()));
services.AddSingleton<IRepository<Category>>(sp =>
    new CachingRepositoryProxy<Category>(sp.GetRequiredService<InMemoryRepository<Category>>()));
services.AddSingleton<IRepository<Operation>>(sp =>
    new CachingRepositoryProxy<Operation>(sp.GetRequiredService<InMemoryRepository<Operation>>()));

// Factories
services.AddSingleton<IBankAccountFactory, BankAccountFactory>();
services.AddSingleton<ICategoryFactory, CategoryFactory>();
services.AddSingleton<IOperationFactory, OperationFactory>();

// Facades
services.AddSingleton<IAccountFacade, AccountFacade>();
services.AddSingleton<ICategoryFacade, CategoryFacade>();
services.AddSingleton<IOperationFacade, OperationFacade>();

// Services
services.AddSingleton<IBalanceService, BalanceService>();
services.AddSingleton<IAnalyticsService, AnalyticsService>();

// Import/Export
services.AddSingleton<IOperationImporter, CsvOperationImporter>();
services.AddSingleton<IOperationImporter, JsonOperationImporter>();
services.AddSingleton<IOperationImporterFactory, OperationImporterFactory>();
services.AddSingleton<OperationExporter>();

// Visitors
services.AddSingleton<IOperationExportVisitor, CsvOperationVisitor>();
services.AddSingleton<IOperationExportVisitor, JsonOperationVisitor>();

var sp = services.BuildServiceProvider();

// Запуsск меню
new ConsoleApp(sp).Run();