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
            var dummyProducts = new List<Product>
            {

                new Product { Name = "X1", Categories = new List<string>() { "Online", "Kiosk", "POS"},
                    Metadata = new Dictionary<string, string>()
                    {
                        { "consumer-tracking", "limited" },
                        { "facial-recognition", "occulus" },
                        { "ai-model", "text-to-text transfer transformer" }
                    }},
                new Product { Name = "X2", Categories = new List<string>() {"POS"},
                    Metadata = new Dictionary<string, string>()
                    {
                        { "consumer-tracking", "none" }
                    }
                },
                new Product { Name = "X3", Categories = new List<string>() {"Kiosk"},
                    Metadata = new Dictionary<string, string>()
                    {
                        { "consumer-tracking", "full" }
                    }
                }

            };

            return Task.FromResult(dummyProducts as IEnumerable<Product>);
        }

        public Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchCriteria criteria)
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