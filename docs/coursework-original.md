# Кочарян Элина Самвеловна 
# БПИ-244-2

---

# HSEBank.Finance

---

## Возможности
- Счета: создание, переименование, удаление, просмотр баланса 
- Категории (доход/расход): создание, переименование, удаление 
- Операции: добавление, редактирование, удаление; авто-коррекция баланса  
- Импорт операций: **CSV/JSON** (Template Method + цепочка валидации)  
- Экспорт операций: **CSV/JSON** (Visitor)  
- Аналитика: доход/расход/итог за период, группировка по категориям.  
- Пересчёт баланса счёта по всем операциям.  
- Добавила защиту от будущих дат и расходов при недостаточном балансе

---

## Структура проектов
- **Solution: HSEBank.Finance**
  - **HSEBank.Console**
    - **App/**
      - **Menu/**
      - `ConsoleApp.cs`
      - `IdFormatter.cs`
    - `Program.cs`
  - **HSEBank.Infrastructure**
    - **Repositories/**
      - `CachingRepositoryProxy.cs`
      - `InMemoryRepository.cs`
  - **HSEBank.Logic**
    - **Application/**
      - **Analytics/**
      - **Command/**
      - **Decorator/**
      - **Exporting/**
      - **Importing/**
      - **Interfaces/**
      - **Service/**   
    - **Domain/**
      - **Entities/**
      - **Enums/**
      - **Facades/**
      - **Factories/**
      - **Interfaces/**

 ---

 # Как работает меню

- **Ввод и проверки (`ConsolePrompts`)**
  - `Ask / AskInt / AskDecimal / AskType` — безопасный ввод.
  - `AskDateNotFuture` — не принимает дату из будущего.
  - `TrySelectAccount` — если нет счетов, сообщает и возвращает в меню.
  - `SelectCategory` — при отсутствии подходящих категорий создаёт дефолтную («Прочие доходы/расходы»).
  - `TrySelectOperation` — выбор операции из списка.
 
- **Действия (`MenuActions`)**
  1. **Создать счёт** — имя + начальный баланс; печать короткого GUID; замер времени.
  2. **Создать категорию** — имя + тип (`Income`/`Expense`).
  3. **Добавить операцию** — выбор счёта/категории, сумма, дата (не в будущем), описание; для расхода проверка «недостаточно средств».
  4. **Импорт (CSV/JSON)** — импортёр выбирается по расширению; строки проходят цепочку валидации; операции добавляются в счёт.
  5. **Экспорт (CSV/JSON)** — формирует контент и сохраняет `export-ops.csv` / `export-ops.json`.
  6. **Редактировать операцию** — выбор операции, ввод новых полей; пересчёт дельты и баланса с проверкой на минус.
  7. **Пересчитать баланс** — пересчёт баланса счёта по всем операциям.
  8. **Показать счета** — список счетов с балансами; ID в коротком виде.
  9. **Аналитика** — запрашивает период; выводит Доход / Расход / Итог и разрез по категориям.
  0. **Выход** — завершение программы.
 
- **Утилиты**
  - `TimedCommand` — печатает время выполнения (в миллисекундах).
  - `IdFormatter.Short` — выводит короткий GUID (`ABCD…EF12`).
	- 
 ---

 ## Паттерны (8 паттернов)
 
| Паттерн | Где реализован | Зачем |
|---|---|---|
| **Facade** | `AccountFacade`, `CategoryFacade`, `OperationFacade` | Единая точка сценариев над сущностями. |
| **Factory (Method/Simple)** | `*Factory` в `Domain/Factories` | Инкапсуляция создания и валидации. |
| **Command** | `CreateAccountCommand`, `AddOperationCommand`, … | Инкапсуляция сценариев; легко декорировать. |
| **Decorator** | `TimedCommand<TResult>` | Замер времени выполнения команд. |
| **Template Method** | `OperationImporterTemplate` | Общий алгоритм импорта; парсинг у наследников. |
| **Visitor** | `CsvOperationVisitor`, `JsonOperationVisitor` | Разные форматы экспорта без правок структуры. |
| **Proxy** | `CachingRepositoryProxy<T>` | Кэш `GetById/GetAll` поверх репозитория. |
| **Chain of Responsibility** | `Importing.Validation.*Rule` | Гибкая валидация строк импорта цепочкой правил. |
 
**Дополнительно:** Repository — `IRepository<T>` + `InMemoryRepository<T>`. DI/IoC — `Microsoft.Extensions.DependencyInjection`.
 
---
 
## SOLID 
- **S**: сущности про данные; фасады — сценарии; импортёры — импорт.  
- **O**: новые форматы импорта/экспорта добавляются классами, без правок базовой логики.  
- **L**: реализации `IRepository<T>` взаимозаменяемы (InMemory/Proxy).  
- **I**: мелкие контракты (`IAccountFacade`, `ICategoryFacade`, `IOperationFacade`, фабрики и т.д.).  
- **D**: зависимости через интерфейсы + DI.
 
## GRASP 
- **Controller**: фасады принимают сценарии.  
- **Information Expert**: расчёты там, где данные (facade/services/entities).  
- **Low Coupling / High Cohesion**: слои и роли разделены.  
- **Creator**: фабрики создают сущности.  
- **Indirection**: DI и Proxy уменьшают связность.  
- **Polymorphism/Protected Variations**: импортёры/визиторы/правила валидации подменяемы.
 
---

## Форматы данных
 
### CSV (первая строка — заголовок)
 
type,category,amount,date,description
Expense,Кафе,350.00,2025-10-01,капучино
Income,Зарплата,50000,2025-10-05,октябрь
 
- `type`: `Income` \| `Expense`  
- `date`: `yyyy-MM-dd` (InvariantCulture)
 
### JSON
```json
[
  { "type": "Expense", "category": "Кафе", "amount": 350.00, "date": "2025-10-01", "description": "капучино" },
  { "type": "Income",  "category": "Зарплата", "amount": 50000, "date": "2025-10-05" }
]