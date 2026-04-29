using Sparcpoint.Models.DTOs;
using Sparcpoint.Models.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Services
{
    public interface IProductService
    {
        Task<int> AddProductAsync(CreateProductRequestDto request);

        Task<IEnumerable<Product>> SearchProductAsync(SearchProductRequestDto request);

        Task<Product> GetProductByIdAsync(int productId);
    }
}
