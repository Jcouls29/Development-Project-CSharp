using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(CreateProductRequest req);
        Task<List<Product>> GetProducts();
    }
}
