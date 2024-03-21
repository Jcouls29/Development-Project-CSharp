using Sparcpoint.Dto;
using Sparcpoint.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IProductService
    {
        Task AddProduct(ProductDto product);
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> SearchProducts(string searchTerm);
        Task AddToInventory(int productId, InventoryTransactionDto inventory);
        Task<int> Count(string input, CountType countType);
        Task<bool> RemoveTransaction(int id);
        Task<bool> AddProductToInventory(int id, InventoryTransactionDto inventory);
        void RemoveProduct(List<int> id);
    }
}
