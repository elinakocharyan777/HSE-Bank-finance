namespace HSEBank.Logic.Application.Importing;

public class OperationImporterFactory : IOperationImporterFactory
{
    private readonly IEnumerable<IOperationImporter> _importers;

    public OperationImporterFactory(IEnumerable<IOperationImporter> importers)
        => _importers = importers;

    public IOperationImporter ResolveByExtension(string pathOrExt)
        => _importers.FirstOrDefault(i => i.CanHandle(pathOrExt))
           ?? throw new InvalidOperationException($"No importer for '{pathOrExt}'.");
}