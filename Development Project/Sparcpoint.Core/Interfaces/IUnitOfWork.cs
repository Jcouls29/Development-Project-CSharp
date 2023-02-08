namespace Sparcpoint.Interfaces
{
    public interface IUnitOfWork
    {
        IProductsRepository ProductsRepository { get; }
    }
}
