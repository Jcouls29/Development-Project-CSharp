using Business.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Business.Services;

public class ProductService : IProductService
{
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    private readonly IProductRepository _productRepository;

    public async Task<List<Product>> GetProducts() => await _productRepository.GetProducts();

    public async Task<List<Product>> GetProductsWithMetadata(List<Metadata> metadatas) => await _productRepository.GetProductsWithMetadata(metadatas);

    public async Task<Product> AddProduct(Product product) => await _productRepository.AddProduct(product);

    public async Task<List<Product>> AddProducts(List<Product> products) => await _productRepository.AddProducts(products);
}