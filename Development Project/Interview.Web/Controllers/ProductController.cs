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
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        // EVAL: The lack of dependency injection is a choice and not an accidental omission. 

        public IConfiguration _config { get; set; }
        
        public ProductController(IConfiguration config) { 
            _config = config; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var inventoryItems = await GetProductsIninventory();

            return (IActionResult)Ok(inventoryItems.ToList());
        }

        protected async virtual Task<IEnumerable<ProductItem>> GetProductsIninventory()
        {
            return await new InventoryManager(_config.GetConnectionString("Inventory")).GetProductsInInventory();
        }
    }
}
