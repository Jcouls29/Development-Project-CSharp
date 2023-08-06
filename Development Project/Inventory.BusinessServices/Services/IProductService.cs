using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inventory.BusinessServices.Services
{
    public interface IProductService
    {
        public Task<HttpResponseMessage> AddProductAsync(Product product);
        public Task<Product> DeleteProductAsync(Product product);
        public Task<Product> GetProductAsync(int productId);
        public List<Product> GetProductAssosicationsAsync(int instanceId);
        public Task<ProductSearch> SearchProductAsync(int productId);

        // Transactions
        public Task<HttpResponseMessage> AddTransactionAsync(InventoryTransaction product);
        public Task<Product> BulkTransactionAsync(List<Product> products);
        public Task<int> DeleteTransactionAsync(int transactionId);
    }
}