using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Services;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public ProductController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            try
            {
                int instanceId = await _inventoryService.CreateProductAsync(request);
                return StatusCode(201, new { InstanceId = instanceId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchRequest request)
        {
            try
            {
                var products = await _inventoryService.SearchProductsAsync(request);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("{instanceId}/inventory")]
        public async Task<IActionResult> GetInventoryCount(int instanceId)
        {
            try
            {
                var count = await _inventoryService.GetInventoryCountAsync(instanceId);
                return Ok(new { InstanceId = instanceId, InventoryCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetTotalInventoryBySearch([FromQuery] ProductSearchRequest request)
        {
            try
            {
                var totalCount = await _inventoryService.GetInventoryCountBySearchAsync(request);
                return Ok(new { TotalInventory = totalCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("{instanceId}/inventory/transactions")]
        public async Task<IActionResult> GetInventoryTransactions(int instanceId)
        {
            try
            {
                var transactions = await _inventoryService.GetInventoryTransactionsAsync(instanceId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("inventory/transactions")]
        public async Task<IActionResult> ProcessInventoryTransactions([FromBody] List<InventoryTransactionRequest> requests)
        {
            try
            {
                await _inventoryService.ProcessInventoryTransactionsAsync(requests);
                return Ok(new { Message = "Transactions processed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("{instanceId}/inventory/transactions/{transactionId}/undo")]
        public async Task<IActionResult> UndoInventoryTransaction(int instanceId, int transactionId)
        {
            try
            {
                await _inventoryService.UndoInventoryTransactionAsync(instanceId, transactionId);
                return Ok(new { Message = $"Transaction {transactionId} undone successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
