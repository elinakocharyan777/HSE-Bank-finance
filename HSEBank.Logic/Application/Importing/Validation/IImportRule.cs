namespace HSEBank.Logic.Application.Importing.Validation;

public interface IImportRule
{
    IImportRule SetNext(IImportRule next);
    bool Validate(OperationImportRow row);
}