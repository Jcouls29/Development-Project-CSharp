using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInventoryTransactions()
        {
            var result = await _inventoryService.GetAllInventoryTransactions();
            return Ok(result);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetInventoryForProduct(int productId)
        {
            var quantity = await  _inventoryService.GetInventoryForProduct(productId);
            return Ok(quantity);
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryByMetadata(KeyValuePair<string, string> metadata)
        {
            var quantity = await _inventoryService.GetInventoryByMetadata(metadata);
            return Ok(quantity);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductInventory(int productId, int newInventoryCount)
        {
            await _inventoryService.UpdateProductInventory(productId, newInventoryCount);
            return Ok();
        }

        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> RollbackInventoryUpdate(int transactionId)
        {
            await _inventoryService.RollbackInventoryUpdate(transactionId);
            return Ok();
        }
    }
}
