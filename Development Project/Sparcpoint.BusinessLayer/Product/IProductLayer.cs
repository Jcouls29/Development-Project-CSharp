using Sparcpoint.Models;
using Sparcpoint.Models.DomainDto.Product;
using Sparcpoint.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.BusinessLayer.Product
{
    public interface IProductLayer
    {
        Task<Products> AddProduct(ProductDomain product);
        Task<List<Products>> SearchProduct(FilterParam filterModel);
    }
}