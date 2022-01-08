using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Models;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private IProductService _productService;
        private IValidationService _validationService;

        public ProductController(IProductService productService, IValidationService validationService)
        {
            _productService = productService;
            _validationService = validationService;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetProducts();
            return Ok(products);
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductRequest req)
        {
            var prodValidation = _validationService.ProductIsValid(req);

            if (!prodValidation.IsValid)
            {
                return BadRequest($"{prodValidation.InvalidMessage}");
            }

            await _productService.CreateProductAsync(req);
            return Ok();
        }

        /// <summary>
        /// Searches products
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts(ProductSearchRequest req)
        {
            var searchalidation = _validationService.SearchIsValid(req);

            if (!searchalidation.IsValid)
            {
                return BadRequest($"{searchalidation.InvalidMessage}");
            }

            var results = await _productService.SearchProducts(req);
            return Ok(results);
        }


        /// <summary>
        /// Gets Attributes for a product
        /// </summary>
        [HttpGet("{productId}/attributes")]
        public async Task<IActionResult> GetAttributesForProduct(int productId)
        {
            if (productId == null)
            {
                return BadRequest("You must provide a product id");
            }

            var result = await _productService.GetAttributesForProduct(productId);
            return Ok(result);
        }

        /// <summary>
        /// Adds a new attribute to a product
        /// </summary>
        [HttpPost("{productId}/attributes")]
        public async Task<IActionResult> AddAttributesToProduct(int productId, [FromBody] Dictionary<string, string> attributes)
        {
            if (attributes == null)
            {
                return BadRequest("You must include at least one attribute");
            }

            await _productService.AddAttributesToProduct(productId, attributes.ToList());
            return Ok();
        }

        /// <summary>
        /// Adds product to a category
        /// </summary>
        [HttpPost("{productId}/categories")]
        public async Task<IActionResult> AddProductToCategories(int productId, List<int> categories)
        {
            if (!categories.Any())
            {
                return BadRequest("You must include at least one category");
            }

            await _productService.AddProductToCategories(productId, categories);
            return Ok();
        }
    }
}
