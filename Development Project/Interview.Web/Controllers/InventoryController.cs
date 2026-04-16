using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Application.Dtos.Requests;
using Sparcpoint.Inventory.Application.Interfaces;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        [HttpPost("AddInventory")]
        public async Task<IActionResult> AddInventory([FromBody] InventoryRequest request)
        {
            await _service.AddInventory(request.ProductIds, request.Quantity);
            return Ok();
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetInventory(int productId)
        {
            var result = await _service.GetInventory(productId);
            return Ok(result);
        }

        [HttpDelete("transaction/{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            await _service.DeleteTransaction(id);
            return Ok();
        }
    }
}