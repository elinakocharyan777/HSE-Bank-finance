namespace HSEBank.Logic.Application.Importing.Validation;

public class AmountPositiveRule : ImportRuleBase
{
    protected override bool Check(OperationImportRow row) => row.Amount > 0;
}