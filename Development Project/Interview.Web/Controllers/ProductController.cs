using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sparcpoint.Inventory.Data;
using Sparcpoint.Product;
using Sparcpoint.Product.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    
    public class ProductController : Controller
    {
        // EVAL: The lack of dependency injection is a choice and not an accidental omission. 
        // EVAL: The lack of interfaces is also a deliberate choice. There's simple no need for them in this project
        // EVAL: Using a document-style approach rather than trying to emulate a schemaless construct in a relational model.
        public IConfiguration _config { get; set; }
        
        public ProductController(IConfiguration config) { 
            _config = config; 
        }

        [HttpGet]
        [Route("api/v1/products")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var inventoryItems = await GetProductsIninventory();
                return (IActionResult)Ok(inventoryItems.ToList());
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("api/v1/products/{productId}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            try
            {
                var product = await GetProductById(productId);
                return (IActionResult)Ok(product);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("api/v1/products")]
        public async Task<IActionResult> PutProduct(ProductItem productItem)
        {
            try
            {
                await AddNewProduct(productItem);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected async Task AddNewProduct(ProductItem productItem)
        {
            await new InventoryManager(_config.GetConnectionString("Inventory")).AddNewProduct(productItem);
        }

        protected async Task<ProductItem> GetProductById(int productId)
        {
            return await new InventoryManager(_config.GetConnectionString("Inventory")).GetProductById(productId);
        }

        protected async virtual Task<IEnumerable<ProductItem>> GetProductsIninventory()
        {
            return await new InventoryManager(_config.GetConnectionString("Inventory")).GetProductsInInventory();
        }
    }
}
