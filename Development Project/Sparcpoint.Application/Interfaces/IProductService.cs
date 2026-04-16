using Sparcpoint.Application.DTOs;
using Sparcpoint.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Interfaces
{
    public interface IProductService
    {
        Task<int> CreateProduct(CreateProductRequest request);
        Task<IEnumerable<Product>> SearchProducts(ProductSearchRequest request);
    }
}
