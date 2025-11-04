namespace HSEBank.Logic.Application.Importing.Validation;

public class CategoryNotEmptyRule : ImportRuleBase
{
    protected override bool Check(OperationImportRow row) =>
        !string.IsNullOrWhiteSpace(row.Category);
}