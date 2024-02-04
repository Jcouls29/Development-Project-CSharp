using Sparcpoint.Products.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Sparcpoint.Exceptions;

namespace Sparcpoint.Products.Domain
{
    public class ProductManager
    {
        private string _connectionString {  get; set; }
        private ProductDataManager _productDataManager { get; set; } 
        public ProductManager() { }

        public ProductManager(string connectionString)
        {
            _connectionString = connectionString;
            _productDataManager = new ProductDataManager(_connectionString);
        }

        // EVAL: I'm keeping public methods as a simple passthroughs with exception handling just to make my life easier
        // In practice, various management/orchestration tasks could go here such as cache-aside,
        // keeping the controller/handler from having to know about any of that
        public async Task<Product> GetProductById(int productId)
        {
            try
            {
                return await QueryProductById(productId);
            }
            catch (System.NullReferenceException ex)
            {
                // EVAL: If you tell the warehouse to get an item that doesn't exist then something has gone wrong.
                // The request never should have been possible and the fact that it can happen should be fixed
                throw new ItemMissingException("Product", productId.ToString(), ex);
            }
            //Dapper throws an InvalidOperationException if you QuerySingle and no records return
            catch (InvalidOperationException ex)
            {
                // EVAL: If you tell the warehouse to get an item that doesn't exist then something has gone wrong.
                // The request never should have been possible and the fact that it can happen should be fixed
                throw new ItemMissingException("Product", productId.ToString(), ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsInInventory() {
            return await QueryProductsInInventory();
        }

        protected async virtual Task<Product> QueryProductById(int productId)
        {
            // EVAL: Instead of an unnecessary Interface, the data manager gets created alongside the code that needs it.
            // This increases readability as it becomes far easier to track down the concrete class being used.
            // Plus, Due to CQRS concerns data managers shouldn't be genericized or abstracted between bounded contexts, so there's no need for an interface
            var dataManager = new ProductDataManager(_connectionString);
            return await dataManager.QueryProductById(productId);
        }

        public async Task AddNewProduct(Product product)
        {
            ValidateProduct(product);
            await QueryAddNewProduct(product);
        }

        public async Task UpdateProduct(Product product)
        {
            ValidateProduct(product);
            await QueryUpdateProduct(product);
        }

        private async Task QueryUpdateProduct(Product product)
        {

            await _productDataManager.UpdateProduct(product);
        }

        protected async virtual Task QueryAddNewProduct(Product product)
        {
            await _productDataManager.AddNewProduct(product);
        }

        protected virtual void ValidateProduct(Product product)
        {
            List<string> missingParameters = new List<string>();

            // There may be a more "elegant" way of doing this but I'm a big fan of being explicit
            if (string.IsNullOrWhiteSpace(product.Manufacturer))
            {
                missingParameters.Add(product.Manufacturer);
            }
            if (string.IsNullOrWhiteSpace(product.ModelName))
            {
                missingParameters.Add(product.ModelName);
            }
            if (string.IsNullOrWhiteSpace(product.Description))
            {
                missingParameters.Add(product.Description);
            }
            if (missingParameters.Any())
            {
                throw new ParameterRequiredException(missingParameters);
            }
        }

        protected async virtual Task<IEnumerable<Product>> QueryProductsInInventory()
        {
            return await _productDataManager.QueryProductsInInventory();
        }

        protected async virtual Task<IEnumerable<Product>> QueryAllInventoryItems() { 
            return await _productDataManager.QueryProductsWithInventoryItems();
        }
    }
}
