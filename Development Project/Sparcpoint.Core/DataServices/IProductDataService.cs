using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
    public interface IProductDataService
    {
        Task<List<Product>> GetProducts();
        Task<Product> CreateProductAsync(Product newProduct);
    }
}
