using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sparcpoint.Models.DTOs;

namespace Sparcpoint.Services
{
    public interface IProductService
    {
        Task CreateProduct(ProductDto product);
    }
    public class ProductService : IProductService
    {
        public ProductService() { }

        public Task CreateProduct(ProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}
