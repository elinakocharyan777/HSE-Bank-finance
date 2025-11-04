namespace HSEBank.Logic.Application.Importing;

public interface IOperationImporter
{
    string Id { get; }     // "csv", "json"
    bool CanHandle(string pathOrExt);       // по расширению
    int Import(string filePath, Guid accountId); // возвращает сколько строк импортировано
}