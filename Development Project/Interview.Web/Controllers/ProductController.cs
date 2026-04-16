using Interview.Web.Extensions;
using Interview.Web.Models;
using Interview.Web.Validators;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // EVAL: Get product by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await this._productService.GetByIdAsync(id);
            
            if (product == null)
            {
                return this.NotFound();
            }
            
            // EVAL: Projection
            var response = new ProductGetByIdResponse
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ImageUris = GetItems(product.ProductImageUris),
                ValidSkus = GetItems(product.ValidSkus),
                CreatedTimestamp = product.CreatedTimestamp,
                Attributes = product.Attributes?.ToDictionary(a => a.Key, a => a.Value) ?? new Dictionary<string, string>()
            };

            return this.Ok(response);
        }

        // EVAL: Create product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
        {
            // EVA: Validation
            var validator = new ProductCreateRequestValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                return result.Errors.ToBadRequest();
            }

            var newProduct = await this._productService.AddAsync(new Product
            {
                Name = request.Name,
                Description = request.Description,
                ProductImageUris = string.Join(",", request.ImageUris),
                ValidSkus = string.Join(",", request.ValidSkus),
                Attributes = request.Attributes.Select(a => new ProductAttribute
                {
                    Key = a.Key,
                    Value = a.Value
                })
            });

            // EVAL: Projection
            var response = new ProductCreateResponse
            {
                InstanceId = newProduct.InstanceId,
                Name = newProduct.Name,
                Description = newProduct.Description,
                ImageUris = GetItems(newProduct.ProductImageUris),
                Skus = GetItems(newProduct.ValidSkus),
                CreatedTimestamp = newProduct.CreatedTimestamp
            };

            return this.CreatedAtAction(nameof(GetProductById), new { id = response.InstanceId }, response);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var products = await this._productService.SearchAsync(request);

            // EVAL: Projection
            var response = products.Select(product => new ProductSearchResponse
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ImageUris = GetItems(product.ProductImageUris),
                Skus = GetItems(product.ValidSkus),
                CreatedTimestamp = product.CreatedTimestamp
            });

            return this.Ok(response);
        }

        private static IEnumerable<string> GetItems(string items, char separator = ',')
        {
            if (items == null)
            {
                return Enumerable.Empty<string>();
            }

            return items.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
