using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.Interfaces;
using Sparcpoint.Core.Models;
using System.Threading.Tasks;

[Route("api/v1/inventory")]
public class InventoryController : Controller
{
    private readonly IInventoryManagementService _inventoryManagementService;

    public InventoryController(IInventoryManagementService inventoryManagementService)
    {
        _inventoryManagementService = inventoryManagementService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToInventory([FromBody] InventoryItem item)
    {
        await _inventoryManagementService.AddToInventoryAsync(item);
        return new CreatedResult(nameof(AddToInventory), item);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveFromInventory([FromBody] InventoryItem item)
    {
        await _inventoryManagementService.RemoveFromInventoryAsync(item);
        return new OkResult();
    }

    [HttpGet("count/{productId}")]
    public async Task<IActionResult> GetInventoryCount(int productId)
    {
        try
        {
            int count = await _inventoryManagementService.GetInventoryCountAsync(productId);
            return Ok(count);
        }
        catch (System.Exception)
        {
            return StatusCode(500, "Internal Server Error, please try back or contact customer service for immediate assistance.");
        }
    }
}