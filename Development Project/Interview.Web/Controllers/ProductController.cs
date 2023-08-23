using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Application.Services;
using Sparcpoint.Inventory.Domain.Entities.Instances;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service) 
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // NOTE: Sample Action
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return await ErrorAsync(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var result = await _service.GetAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return await ErrorAsync(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                var result = await _service.AddProductAsync(product);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return await ErrorAsync(ex);
            }
        }

        [HttpPost("{id}/categories")]
        public async Task<IActionResult> AddProductCategory(int id, int categoryId)
        {
            try
            {
                var result = await _service.AddProductCategoryAsync(id, categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return await ErrorAsync(ex);
            }
        }

        [HttpPost("{id}/attributes")]
        public async Task<IActionResult> AddProductAttribute(int id, [FromBody] InstanceAttribute attribute)
        {
            try
            {
                var result = await _service.AddProductAttributeAsync(id, attribute.Key, attribute.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return await ErrorAsync(ex);
            }
        }

        private Task<IActionResult> ErrorAsync(Exception ex)
        {
            return Task.FromResult((IActionResult)Ok(new { Exception = nameof(ex), ex.Message }));
        }
    }
}
