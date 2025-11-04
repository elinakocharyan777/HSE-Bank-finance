namespace HSEBank.Logic.Application.Importing;

public interface IOperationImporterFactory
{
    IOperationImporter ResolveByExtension(string pathOrExt);
}