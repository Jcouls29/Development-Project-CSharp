using Interview.Web.Model;
using Interview.Web.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IProductRepository _productRepo;

        public ProductController(IInventoryRepository inventoryRepo, IProductRepository productRepo)
        {
            _inventoryRepo = inventoryRepo;
            _productRepo = productRepo;
        }

        /// <summary>
        /// Add product and category details
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost("addProduct")]
        public Task AddProduct([FromBody] Product product)
        {
            return _productRepo.Add(product);
        }


        /// <summary>
        /// Add product to the inventory
        /// </summary>
        /// <param name="inventoryTransactions"></param>
        /// <returns>void</returns>

        [HttpPost("addInventory")]
        public Task AddInventory([FromBody] InventoryTransactions inventoryTransactions)
        {
            return _inventoryRepo.Add(inventoryTransactions);
        }

        /// <summary>
        /// get the  count of the products or type catgory from the Inventory
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        [HttpGet("inventoryCount")]
        public Task<int> GetProductInventoryCount(int instanceId = 0, string categoryType = "")
        {
            var inventoryTransactions = new InventoryTransactions { ProductInstanceId = instanceId, TypeCategory = categoryType };
            return _inventoryRepo.GetProductInventoryCount(inventoryTransactions);
        }


        

        /// <summary>
        /// Search Product based on Description or Category
        /// </summary>
        /// <param name="decription"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        [HttpGet("searchProduct")]
        public Task<List<Product>> SearchProduct(string description = "", string categoryName = "")
        {
            var product = new Product { CategoryName = categoryName, Description = description };
            return _productRepo.SearchProduct(product);
        }

        /// <summary>
        /// Delete product from the inventory
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>

        [HttpDelete("{id}")]
        public Task<bool> DeleteProductsfromInventory(int id)
        {
            var inventoryTransactions = new InventoryTransactions { ProductInstanceId = id };
            return _inventoryRepo.Remove(inventoryTransactions);

        }


    }
}
