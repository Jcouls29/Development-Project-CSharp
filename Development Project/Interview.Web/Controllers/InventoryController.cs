using Interview.Web.Models;
using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        // POST: api/inventory/add
        [HttpPost("add")]
        public async Task<IActionResult> AddInventory([FromBody] AddInventoryRequest request)
        {
            try
            {
                await _inventoryService.AddInventoryAsync(request);

                return Ok(new
                {
                    Message = "Inventory added successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Adding Inventoey");
                return StatusCode(500, "Internal Server Error");

            }
        }

        // POST: api/inventory/remove
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveInventory([FromBody] RemoveInventoryRequest request)
        {
            try
            {
                await _inventoryService.RemoveInventoryAsync(request);

                return Ok(new
                {
                    Message = "Inventory removed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Removing Inventoey");
                return StatusCode(500, "Internal Server Error");

            }

        }

        // GET: api/inventory/{productId}
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetInventory(int productId)
        {
            try
            {
                var result = await _inventoryService.GetInventoryCountAsync(productId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get Inventoey");
                return StatusCode(500, "Internal Server Error");

            }

        }
    }
}