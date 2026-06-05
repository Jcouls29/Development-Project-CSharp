using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Controllers;

[ApiController]
[Route("api/v1/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpPost]
    public async Task<IActionResult> AddInventory([FromBody] AddInventoryRequest request)
    {
        var transactionId = await _inventoryService.AddAsync(request);
        // EVAL: Returning the TransactionId gives the caller a handle to reference this
        // specific transaction if they need to undo it via DELETE later.
        return Ok(new { TransactionId = transactionId });
    }

    [HttpDelete("{transactionId}")]
    public async Task<IActionResult> RemoveTransaction(int transactionId)
    {
        await _inventoryService.RemoveTransactionAsync(transactionId);
        // EVAL: 204 No Content is the correct REST response for a successful DELETE
        // that returns no body.
        return NoContent();
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount([FromQuery] InventoryCountRequest request)
    {
        var count = await _inventoryService.GetCountAsync(request);
        return Ok(new { Count = count });
    }
}
