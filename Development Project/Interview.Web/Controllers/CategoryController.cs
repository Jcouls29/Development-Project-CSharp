using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _Categories;

        public CategoryController(ICategoryRepository categories)
        {
            _Categories = categories;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _Categories.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> GetById(int categoryId)
        {
            var category = await _Categories.GetByIdAsync(categoryId);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Name))
                return BadRequest("Name is required.");

            try
            {
                var categoryId = await _Categories.CreateAsync(request);
                var created = await _Categories.GetByIdAsync(categoryId);
                return CreatedAtAction(nameof(GetById), new { categoryId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
