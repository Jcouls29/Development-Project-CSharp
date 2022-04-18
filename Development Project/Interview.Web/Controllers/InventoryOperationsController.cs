using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Commands;
using Interview.Web.Domain.Dto;
using Interview.Web.Queries;
using MediatR;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventoryOperations")]
    public class InventoryOperationsController : Controller
    {
        private readonly IMediator _mediator;

        public InventoryOperationsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<InventoryOperationData>> CreateInventoryOperation([FromBody] InventoryOperationData inventoryOperationData)
        {
            return await _mediator.Send(new AddInventoryOperationCommand(inventoryOperationData));
        }
    }
}

