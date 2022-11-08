using Sparcpoint.Models.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Abstract
{
    public interface IProductService
    {
        Task<int> CreateBaseProductServiceAsync(BaseProduct addProduct, string consumerId);

        Task<List<BaseProduct>> GetBaseProductsServiceAsync(BaseProduct searchProduct, string consumerId);
    }
}
