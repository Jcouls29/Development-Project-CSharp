using Interview.Entities;
using Interview.Services.Interfaces;
using Inteview.Repository;

namespace Interview.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public int Add(Product product)
        {
            return _productRepository.AddProduct(product);
        }
    }
}