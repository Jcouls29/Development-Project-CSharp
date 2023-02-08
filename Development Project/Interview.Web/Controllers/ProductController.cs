using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Sparcpoint.Application.Abstracts;
using Sparcpoint.Core.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        // NOTE: Sample Action
        private readonly IProductService _productService;
        private readonly IProductValidationService _productValidationService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IProductValidationService productValidationService, IMapper mapper)
        {

            _productService = productService;
            _productValidationService = productValidationService;
            _mapper = mapper;
        }
        /// <summary>
        /// Get All Products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<Product>> GetAllProducts()
        {
            return await _productService.GetProducts();
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        [Route("create_product")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var validation = _productValidationService.ProductIsValid(request);

            if (!validation.IsValid)
            {
                return BadRequest($"{validation.InvalidMessage}");
            }
            Product product = _mapper.Map<Product>(request);
            await _productService.CreateProductAsync(product);
            return Ok();
        }
        /// <summary>
        /// Adds a new attribute to a product
        /// </summary>
        [HttpPost("{productId}/attributes")]
        public async Task<IActionResult> AddAttributesToProduct(int productId, [FromBody] List<ProductAttributeRequest> attributesRequest)
        {
            if (!attributesRequest.Any())
            {
                return BadRequest("You must include at least one attribute");
            }
            var attributes = _mapper.Map<List<ProductAttributeRequest>, List<ProductAttribute>>(attributesRequest);
            await _productService.AddAttributesToProductAsync(productId, attributes.ToList());
            return Ok();
        }

        /// <summary>
        /// Adds product to a category
        /// </summary>
        [HttpPost("{productId}/categories")]
        public async Task<IActionResult> AddProductToCategories(int productId, List<ProductCategoryRequest> categoriesRequest)
        {
            if (!categoriesRequest.Any())
            {
                return BadRequest("You must include at least one category");
            }
            var categories = _mapper.Map<List<ProductCategoryRequest>, List<ProductCategory>>(categoriesRequest);
            await _productService.AddProductToCategories(productId, categories);
            return Ok();
        }
        /// <summary>
        /// Searches products
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts(string keyword)
        {
            var results = await _productService.SearchProductsAsync(keyword);
            return Ok(results);
        }
    }
}