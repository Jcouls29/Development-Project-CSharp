using DataAccess.Models;

namespace DataAccess.Interfaces;

public interface IProductRepository
{
    public Task<List<Product>> GetProducts();
    public Task<List<Product>> GetProductsWithMetadata(List<Metadata> metadatas);
    public Task<Product> AddProduct(Product product);
    public Task<List<Product>> AddProducts(List<Product> products);
}