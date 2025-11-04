using HSEBank.Logic.Domain.Interfaces;

namespace HSEBank.Infrastructure.Repositories;

public class CachingRepositoryProxy<T> : IRepository<T> where T : class, IEntity
{
    private readonly IRepository<T> _inner;

    private readonly Dictionary<Guid, T> _byId = new();
    private IReadOnlyCollection<T>? _allCache;
    private bool _allValid;

    public CachingRepositoryProxy(IRepository<T> inner) => _inner = inner;

    public void Add(T entity)
    {
        _inner.Add(entity);
        Invalidate(entity.Id);
    }

    public T? GetById(Guid id)
    {
        if (_byId.TryGetValue(id, out var cached)) return cached;
        var e = _inner.GetById(id);
        if (e is not null) _byId[id] = e;
        return e;
    }

    public IReadOnlyCollection<T> GetAll()
    {
        if (_allValid && _allCache is not null) return _allCache;
        _allCache = _inner.GetAll();
        _allValid = true;
        // поддержим синхронность кэша по Id
        _byId.Clear();
        foreach (var e in _allCache) _byId[e.Id] = e;
        return _allCache;
    }

    public bool Remove(Guid id)
    {
        var ok = _inner.Remove(id);
        Invalidate(id);
        return ok;
    }

    public void Update(T entity)
    {
        _inner.Update(entity);
        Invalidate(entity.Id);
    }

    private void Invalidate(Guid id)
    {
        _byId.Remove(id);
        _allValid = false;
        _allCache = null;
    }
}