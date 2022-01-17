using SparcPoint.Inventory.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Probably not an Apt name or location. Just wanted to show the demarkation of layers
namespace Sparcpoint.BL
{
    /// <summary>
    /// 
    /// </summary>
    public class BusinessLayer : IBusinessLayer
    {
        private IAsyncCollection<Product> products;
        public BusinessLayer(IAsyncCollection<Product> productCollection)
        {
            products = productCollection; 
        }

        public Task AddProduct(Product item)
        {
            try
            {
                return products.Add(item);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot add Product. XXXX Reason  XXXX", ex);
            }
        }

        public Task<List<Product>> FindProduct(string searchParameters)
        {
            //return products where the Attributes/ Category name or category atttributes match the one or more  of the parameters.

            return null;
        }

        public Task<List<Product>> FindProductsOfCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetAllProducts()
        {
            throw new NotImplementedException();
        }

        public Task<int> ProductCount()
        {
            throw new NotImplementedException();
        }

        public Task<int> ProductCount(string searchparameter)
        {
            return Task.FromResult(this.FindProduct(searchparameter).GetAwaiter().GetResult().Count());
        }

        public Task<bool> RemoveProduct(Product item)
        {
            throw new NotImplementedException();
        }
    }
}
