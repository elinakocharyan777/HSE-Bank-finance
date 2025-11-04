using HSEBank.Logic.Domain.Interfaces;

namespace HSEBank.Logic.Domain.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    void Add(T entity);
    void Update(T entity);
    bool Remove(Guid id);
    T? GetById(Guid id);
    IReadOnlyCollection<T> GetAll();
}