using Interview.Web.Contracts;
using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IInventoryManagementService _inventoryManagementService;

        public CategoryController(IInventoryManagementService inventoryManagementService)
        {
            _inventoryManagementService = inventoryManagementService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetCategories()
        {
            var categories = await _inventoryManagementService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var category = await _inventoryManagementService.CreateCategoryAsync(request);
            return Created($"/api/v1/categories/{category.CategoryId}", category);
        }
    }
}
