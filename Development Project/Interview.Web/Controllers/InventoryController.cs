using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract;
using Sparcpoint.Implementations;
using Sparcpoint.Models.Requests;
using Sparcpoint.Models.Tables;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventories")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
                _inventoryService = inventoryService;
        }

        [Route("update")]
        [HttpPut]
        public async Task<IActionResult> UpdateProductInventory([FromBody] UpdateInventoryRequest request)
        {
            await _inventoryService.UpdateProductInventoryAsync(request);
            return Ok();
        }
    }
}