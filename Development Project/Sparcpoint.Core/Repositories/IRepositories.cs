using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;

namespace Sparcpoint.Core.Repositories
{
    // EVAL: Repository Pattern - Abstracts data access behind interface for testability and decoupling
    // EVAL: Interface segregation - separate concerns for different domain entities
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int instanceId);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> SearchAsync(string name = null, string description = null,
            IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null,
            int? skip = null, int? take = null);
        Task<int> GetSearchCountAsync(string name = null, string description = null,
            IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null);
        Task<int> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task<decimal> GetCurrentInventoryCountAsync(int productInstanceId);
    }

    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int instanceId);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<int>> GetParentCategoryIdsAsync(int categoryInstanceId);
        Task<IEnumerable<int>> GetChildCategoryIdsAsync(int categoryInstanceId);
        Task<int> AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task UpdateParentCategoryRelationshipsAsync(int categoryInstanceId, IEnumerable<int> parentCategoryIds);
        Task DeleteAsync(int instanceId);
    }

    public interface IInventoryTransactionRepository
    {
        Task<InventoryTransaction> GetByIdAsync(int transactionId);
        Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productInstanceId);
        Task<int> AddAsync(InventoryTransaction transaction);
        Task CompleteTransactionAsync(int transactionId);
        Task<IEnumerable<InventoryTransaction>> GetUncompletedTransactionsAsync();
    }
}