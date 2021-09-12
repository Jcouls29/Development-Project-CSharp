using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.models;

namespace Interview.Web
{
    public interface IProduct
    {
        void AddProduct(ProductDetailsModel pod);
        IEnumerable<ProductDetailsModel> GetAllProducts();
        ProductDetailsModel GetProductById(int id);
       void UpdateProduct(ProductDetailsModel entity);
    }
}
