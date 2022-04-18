using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Commands;
using Interview.Web.Domain.Dto;
using Interview.Web.Queries;
using MediatR;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventories")]
    public class InventoriesController : Controller
    {
        private readonly IMediator _mediator;

        public InventoriesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<InventoryData>> CreateInventory([FromBody] InventoryData inventoryData)
        {
            return await _mediator.Send(new AddInventoryCommand(inventoryData));
        }
    }
}

