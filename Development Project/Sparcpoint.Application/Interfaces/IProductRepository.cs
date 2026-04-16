using Sparcpoint.Application.DTOs;
using Sparcpoint.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<int> InsertProduct(CreateProductRequest request);
        Task<IEnumerable<Product>> SearchProducts(ProductSearchRequest request);
    }
}
