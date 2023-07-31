using Sparcpoint.Entities;
using Sparcpoint.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Services.interfaces
{
    public interface IProductService
    {

        Task<Product> AddProduct(ProductModel productModel);
        Task<Product> UpdateProduct(ProductModel productModel);
        Task<List<Product>> GetAllProduct();
        Task<Product> DeleteProduct(Guid productId);
        Task<bool> ProductExists(Guid productId);

    }
}
