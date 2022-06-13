using Interview.Web.Entities;
using Interview.Web.IRepository;
using Interview.Web.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Product> AddProduct(Product product)
        {
            return await _productRepository.Add(product);
        }
    }
}
