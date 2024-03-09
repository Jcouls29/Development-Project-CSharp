using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Interview.Web.Models;
using Interview.Web.Services;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly ClientsProductService _clientsProductService;

        public ProductController(ClientsProductService clientsProductService)
        {
            _clientsProductService = clientsProductService;
        }

        /// <summary>
        /// Retrieves products based on search criteria.
        /// If no search criteria provided, returns all products.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductSearchModel searchValues)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchValues.SKU) && string.IsNullOrWhiteSpace(searchValues.Name))
                {
                    var allProducts = await _clientsProductService.GetAllProductsAndCorrespondingCategory();
                    return Ok(allProducts);
                }
                else
                {
                    var products = await _clientsProductService.SearchProductsAsync(searchValues);
    
                    if (products == null || products.Count == 0)
                    {
                        return NotFound("No products found matching the criteria");
                    }
    
                    return Ok(products);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving products");
            }
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDetailsModel productModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newProduct = await _clientsProductService.CreateProductAsync(productModel);
                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error creating a new product");
            }
        }

        /// <summary>
        /// Retrieves a product by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _clientsProductService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound("No product found");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving product");
            }
        }

        /// <summary>
        /// Adds a specified quantity of the product to the inventory.
        /// </summary>
        [HttpPost("{id}/add-to-inventory")]
        public async Task<IActionResult> AddToInventory(int id, [FromBody] int quantity)
        {
            try
            {
                await _clientsProductService.AddToInventoryAsync(id, quantity);
                return Ok($"Added {quantity} units to the inventory for product with ID {id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error adding to inventory");
            }
        }

        /// <summary>
        /// Removes a specified quantity of the product from the inventory.
        /// </summary>
        [HttpPost("{id}/remove-from-inventory")]
        public async Task<IActionResult> RemoveFromInventory(int id, [FromBody] int quantity)
        {
            try
            {
                await _clientsProductService.RemoveFromInventoryAsync(id, quantity);
                return Ok($"Removed {quantity} units from the inventory for product with ID {id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error removing from inventory");
            }
        }

        /// <summary>
        /// Retrieves the count of product inventory based on a unique product identifier or metadata.
        /// </summary>
        [HttpGet("inventory/count")]
        public async Task<IActionResult> GetProductInventoryCount([FromQuery] string identifier)
        {
            try
            {
                var inventoryCount = await _clientsProductService.GetProductInventoryCountAsync(identifier);
                return Ok(inventoryCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving product inventory count");
            }
        }
    }
}
