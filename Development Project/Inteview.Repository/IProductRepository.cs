using Interview.Entities;

namespace Inteview.Repository
{
    public interface IProductRepository
    {
        int AddProduct(Product product);
    }
}