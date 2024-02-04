using Sparcpoint.Inventory.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Sparcpoint.Product.Domain
{
    public class InventoryManager
    {
        private string _connectionString {  get; set; }
        public InventoryManager() { }

        public InventoryManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        // EVAL: I'm keeping public methods as a simple passthroughs with exception handling just to make my life easier
        // In practice, various management/orchestration tasks could go here such as cache-aside,
        // keeping the controller/handler from having to know about any of that
        public async Task<ProductItem> GetProductById(int productId)
        {
            var product = await QueryProductById(productId);

            // EVAL: If you tell the warehouse to get an item that doesn't exist then something has gone wrong.
            // The request never should have been possible and the fact that it can happen should be fixed
            return product == null ? throw new Exception($"Product {productId} does not exist") : product;
        }

        public async Task<IEnumerable<ProductItem>> GetProductsInInventory() {
            return await QueryProductsInInventory();
        }

        protected async virtual Task<ProductItem> QueryProductById(int productId)
        {
            // EVAL: Instead of an unnecessary Interface, the data manager gets created alongside the code that needs it.
            // This increases readability as it becomes far easier to track down the concrete class being used.
            // Plus, Due to CQRS concerns data managers shouldn't be genericized or abstracted between bounded contexts, so there's no need for an interface
            var dataManager = new InventoryDataManager(_connectionString);
            return await dataManager.QueryProductById(productId);
        }

        public async Task AddNewProduct(ProductItem productItem)
        {
            await new InventoryDataManager(_connectionString).AddNewProduct(productItem);
        }

        protected async virtual Task<IEnumerable<ProductItem>> QueryProductsInInventory()
        {
            var dataManager = new InventoryDataManager(_connectionString);
            return await dataManager.QueryProductsInInventory();
        }

        protected async virtual Task<IEnumerable<ProductItem>> QueryAllInventoryItems() { 
            var dataManager = new InventoryDataManager(_connectionString);
            return await dataManager.QueryProductsWithInventoryItems();
        }

    }
}
