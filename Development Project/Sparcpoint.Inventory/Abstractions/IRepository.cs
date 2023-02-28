using System.Linq.Expressions;

namespace Sparcpoint.Inventory.Abstractions;

public interface IRepository<T> where T : class
{
    public IQueryable<T> GetAll();

    public Task<IEnumerable<T>> GetAllAsync();

    public T Get(int id);

    public Task<T?> GetAsync(int id);

    public T Add(T t);

    public Task<T> AddAsync(T t);

    public T? Find(Expression<Func<T, bool>> expr);

    public Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> expr);

    public IQueryable<T> FindBy(Expression<Func<T, bool>> expr);

    public Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> expr);

    public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);

    public T? Update(T t, object key);

    public Task<T?> UpdateAsync(T t, object key);
}