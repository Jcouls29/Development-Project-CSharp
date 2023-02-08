using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Application.Abstracts;
using Sparcpoint.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventories")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IInventoryValidationService _inventoryValidationService;
        private readonly IMapper _mapper;

        public InventoryController(IInventoryService inventoryService, IInventoryValidationService inventoryValidationService, IMapper mapper)
        {
            _inventoryService = inventoryService;
            _inventoryValidationService = inventoryValidationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Update inventory for a specific product
        /// </summary>
        [HttpPut("update_inventory")]
        public async Task<IActionResult> UpdateProductInventory([FromBody] InventoryTransactionRequest request)
        {

            ValidationResponse validation = _inventoryValidationService.InventoryIsValid(request);


            if (!validation.IsValid)
            {
                return BadRequest(validation.InvalidMessage);
            }
            InventoryTransaction inventoryTransaction = _mapper.Map<InventoryTransaction>(request);
            await _inventoryService.AddInventoryTransactionAsync(inventoryTransaction);
            return Ok();
        }

        // <summary>
        /// Removes the indicated inventory transaction
        /// </summary>
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> RollbackInventoryUpdate(int transactionId)
        {
            if (transactionId == 0)
            {
                return BadRequest("A transaction id must be provided");
            }

            await _inventoryService.RollbackInventoryTransactionAsync(transactionId);
            return Ok();
        }
        /// <summary>
        /// Search Inventory For Product's Meta Data
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchInventory(string keyword)
        {
            var results = await _inventoryService.SearchInventoryAsync(keyword);
            return Ok(results);
        }

    }
}