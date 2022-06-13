using Interview.Web.Entities;
using Interview.Web.IRepository;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Repository
{
    public class ProductRepository : IProductRepository
    {
        public async Task<Product> Add(Product product)
        {
            return await CreateNewProduct();
        }

        public async Task<Product> CreateNewProduct()
        {
            await Task.Delay(500);// Represents TimeConsuming Add Operation
            Product p = new Product { CreatedTimestamp = DateTime.Now, Description = "Test", InstanceId = 1, Name = "Some Product", ProductImageUris = "/images/p1.png", ValidSkus = "1005" };

            return p;
        }

    }
}
