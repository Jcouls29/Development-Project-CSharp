using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Interview.Web.Services;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetInventoryByProductId(Guid productId)
        {
            var inventory = await _inventoryService.GetInventoryByProductId(productId);
            if (inventory == null)
                return NotFound($"Product '{productId}' was not found.");

            return Ok(inventory);
        }
    }
}
