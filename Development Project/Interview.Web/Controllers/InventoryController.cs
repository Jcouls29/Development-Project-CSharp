using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Sparcpoint.Products.Domain;
using Sparcpoint.Products.Data;

namespace Interview.Web.Controllers
{
    public class InventoryController : Controller
    {
        public IConfiguration _config { get; set; }

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("api/v1/inventory/count/{sku}")]
        public async Task<IActionResult> GetInventoryCount(string sku)
        {
            try
            {
                int itemCount = await GetInventoryCountBySku(sku);
                return (IActionResult)Ok(itemCount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("api/v1/inventory")]
        public async Task<IActionResult> PutProduct([FromBody] InventoryItem inventoryItem)
        {
            try
            {
                await AddNewInventoryItem(inventoryItem);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/v1/inventory")]
        public async Task<IActionResult> PostProduct([FromBody] InventoryItem inventoryItem)
        {
            try
            {
                await UpdateInventoryItem(inventoryItem);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected async virtual Task<int> GetInventoryCountBySku(string sku)
        {
            return await new ProductManager(_config.GetConnectionString("Inventory")).GetInventoryCountBySku(sku);
        }

        protected async virtual Task<IActionResult> AddNewInventoryItem(InventoryItem inventoryItem)
        {
            try
            {
                await new ProductManager(_config.GetConnectionString("Inventory")).AddNewInventoryItem(inventoryItem);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected async virtual Task UpdateInventoryItem(InventoryItem inventoryItem)
        {
            await new ProductManager(_config.GetConnectionString("Inventory")).UpdateInventoryItem(inventoryItem);
        }

    }
}
