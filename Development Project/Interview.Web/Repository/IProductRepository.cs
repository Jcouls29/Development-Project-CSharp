namespace Interview.Web.Repository
{
    /// <summary>
    /// Interface for product repository
    /// </summary>
    public interface IProductRepository
    {
        List<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
