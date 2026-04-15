using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.DTOs;
using Sparcpoint.Core.Repositories;
using Sparcpoint.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    [ApiController]
    // EVAL: API Controller - RESTful endpoints for inventory management using ledger pattern
    // EVAL: Model validation - automatic validation using DataAnnotations
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IInventoryTransactionRepository _transactionRepository;
        private readonly IProductRepository _productRepository;

        // EVAL: Dependency Injection - Constructor injection for loose coupling
        public InventoryController(IInventoryService inventoryService, IInventoryTransactionRepository transactionRepository, IProductRepository productRepository)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        // EVAL: RESTful GET - Get current inventory for a product
        [HttpGet("products/{productId}")]
        public async Task<IActionResult> GetCurrentInventory(int productId)
        {
            // EVAL: Verify product exists
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound("Product not found");

            var currentInventory = await _inventoryService.GetCurrentInventoryAsync(productId);

            return Ok(new
            {
                ProductId = productId,
                ProductName = product.Name,
                CurrentInventory = currentInventory
            });
        }

        // EVAL: RESTful POST - Add inventory (ledger pattern: positive transaction)
        [HttpPost("products/{productId}/add")]
        public async Task<IActionResult> AddInventory(int productId, [FromBody] AddInventoryDto addDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // EVAL: Business operation - add inventory via ledger transaction
                var transactionId = await _inventoryService.AddInventoryAsync(productId, addDto.Quantity, addDto.TypeCategory);

                return Ok(new
                {
                    TransactionId = transactionId,
                    ProductId = productId,
                    QuantityAdded = addDto.Quantity,
                    TypeCategory = addDto.TypeCategory ?? "ADD"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EVAL: RESTful POST - Remove inventory (ledger pattern: negative transaction)
        [HttpPost("products/{productId}/remove")]
        public async Task<IActionResult> RemoveInventory(int productId, [FromBody] RemoveInventoryDto removeDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // EVAL: Business operation - remove inventory via ledger transaction
                var transactionId = await _inventoryService.RemoveInventoryAsync(productId, removeDto.Quantity, removeDto.TypeCategory);

                return Ok(new
                {
                    TransactionId = transactionId,
                    ProductId = productId,
                    QuantityRemoved = removeDto.Quantity,
                    TypeCategory = removeDto.TypeCategory ?? "REMOVE"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EVAL: RESTful POST - Bulk add inventory for multiple products
        [HttpPost("products/add/bulk")]
        public async Task<IActionResult> AddInventoryBulk([FromBody] InventoryBulkAdjustmentDto bulkDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var transactionIds = await _inventoryService.AddInventoryAsync(bulkDto.Adjustments.Select(x => new Sparcpoint.Core.Models.InventoryAdjustment
                {
                    ProductInstanceId = x.ProductInstanceId,
                    Quantity = x.Quantity,
                    TypeCategory = x.TypeCategory
                }));

                return Ok(new
                {
                    TransactionIds = transactionIds,
                    Count = transactionIds.Count()
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EVAL: RESTful POST - Bulk remove inventory for multiple products
        [HttpPost("products/remove/bulk")]
        public async Task<IActionResult> RemoveInventoryBulk([FromBody] InventoryBulkAdjustmentDto bulkDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var transactionIds = await _inventoryService.RemoveInventoryAsync(bulkDto.Adjustments.Select(x => new Sparcpoint.Core.Models.InventoryAdjustment
                {
                    ProductInstanceId = x.ProductInstanceId,
                    Quantity = x.Quantity,
                    TypeCategory = x.TypeCategory
                }));

                return Ok(new
                {
                    TransactionIds = transactionIds,
                    Count = transactionIds.Count()
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EVAL: RESTful POST - Inventory report by metadata and category filters
        [HttpGet("counts/search")]
        public async Task<IActionResult> GetInventoryCountByFilter([FromQuery] string name, [FromQuery] string description,
            [FromQuery] List<int> categoryInstanceIds, [FromQuery] Dictionary<string, string> metadataFilters)
        {
            var totalInventory = await _inventoryService.GetInventoryCountByFilterAsync(name, description, categoryInstanceIds, metadataFilters);
            return Ok(new { TotalInventory = totalInventory });
        }

        [HttpPost("report")]
        public async Task<IActionResult> GetInventoryReport([FromBody] InventoryReportRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var products = await _productRepository.SearchAsync(
                requestDto.Name,
                requestDto.Description,
                requestDto.CategoryInstanceIds,
                requestDto.MetadataFilters,
                null,
                null);

            var reportProducts = new List<ProductDto>();
            decimal total = 0;

            foreach (var product in products)
            {
                var inventoryCount = await _inventoryService.GetCurrentInventoryAsync(product.InstanceId);
                total += inventoryCount;
                reportProducts.Add(new ProductDto
                {
                    InstanceId = product.InstanceId,
                    Name = product.Name,
                    Description = product.Description,
                    ProductImageUris = product.ProductImageUris.ToList(),
                    ValidSkus = product.ValidSkus.ToList(),
                    CreatedTimestamp = product.CreatedTimestamp,
                    Categories = product.Categories.Select(c => new CategoryDto
                    {
                        InstanceId = c.InstanceId,
                        Name = c.Name,
                        Description = c.Description,
                        CreatedTimestamp = c.CreatedTimestamp
                    }).ToList(),
                    Metadata = new Dictionary<string, string>(product.Metadata),
                    CurrentInventoryCount = inventoryCount
                });
            }

            return Ok(new InventoryReportDto
            {
                TotalInventory = total,
                Products = reportProducts
            });
        }

        // EVAL: RESTful POST - Undo transaction (compensating transaction in ledger)
        [HttpPost("transactions/{transactionId}/undo")]
        [HttpPost("transactions/{transactionId}/reverse")]
        public async Task<IActionResult> UndoTransaction(int transactionId)
        {
            try
            {
                // EVAL: Business operation - undo via compensating ledger transaction
                var success = await _inventoryService.UndoTransactionAsync(transactionId);

                if (!success) return NotFound("Transaction not found");

                return Ok(new
                {
                    OriginalTransactionId = transactionId,
                    Status = "Undone"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EVAL: RESTful GET - Get transaction history for a product
        [HttpGet("products/{productId}/transactions")]
        public async Task<IActionResult> GetTransactionHistory(int productId)
        {
            // EVAL: Verify product exists
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound("Product not found");

            var transactions = await _transactionRepository.GetByProductIdAsync(productId);
            var transactionDtos = new List<InventoryTransactionDto>();

            foreach (var transaction in transactions)
            {
                transactionDtos.Add(new InventoryTransactionDto
                {
                    TransactionId = transaction.TransactionId,
                    ProductInstanceId = transaction.ProductInstanceId,
                    Quantity = transaction.Quantity,
                    StartedTimestamp = transaction.StartedTimestamp,
                    CompletedTimestamp = transaction.CompletedTimestamp,
                    TypeCategory = transaction.TypeCategory
                });
            }

            return Ok(new
            {
                ProductId = productId,
                ProductName = product.Name,
                Transactions = transactionDtos
            });
        }

        // EVAL: RESTful GET - Get specific transaction details
        [HttpGet("transactions/{transactionId}")]
        public async Task<IActionResult> GetTransaction(int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null) return NotFound();

            var transactionDto = new InventoryTransactionDto
            {
                TransactionId = transaction.TransactionId,
                ProductInstanceId = transaction.ProductInstanceId,
                Quantity = transaction.Quantity,
                StartedTimestamp = transaction.StartedTimestamp,
                CompletedTimestamp = transaction.CompletedTimestamp,
                TypeCategory = transaction.TypeCategory
            };

            return Ok(transactionDto);
        }
    }
}