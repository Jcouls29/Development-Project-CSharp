using Sparcpoint.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    public class ProductManagementService : IProductManagementService
    {
        // TODO: Implement this service by using the SQL Server Executor (although I am tempted to use EF Core to make it rapid)

        public Task AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        internal Task<Product> GetProductAsync(string name)
        {
            throw new NotImplementedException();
        }

        Task<Product> IProductManagementService.GetProductAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}