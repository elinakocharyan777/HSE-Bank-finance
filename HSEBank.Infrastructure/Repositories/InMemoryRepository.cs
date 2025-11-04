using HSEBank.Logic.Domain.Interfaces;

namespace HSEBank.Infrastructure.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly Dictionary<Guid, T> _store = new();

    public void Add(T entity) => _store[entity.Id] = entity;

    public T? GetById(Guid id) => _store.TryGetValue(id, out var entity) ? entity : null;

    public IReadOnlyCollection<T> GetAll() => _store.Values.ToList().AsReadOnly();

    public bool Remove(Guid id) => _store.Remove(id);

    public void Update(T entity)
    {
        if (!_store.ContainsKey(entity.Id))
            throw new InvalidOperationException("Entity not found");
        _store[entity.Id] = entity;
    }
}