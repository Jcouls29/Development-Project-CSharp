using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory;
using Sparcpoint.Inventory.Models;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _Categories;

        public CategoryController(ICategoryRepository categories)
        {
            _Categories = categories ?? throw new ArgumentNullException(nameof(categories));
        }

        /// <summary>GET /api/v1/categories</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _Categories.GetAllAsync());

        /// <summary>GET /api/v1/categories/{id}</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _Categories.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        /// <summary>POST /api/v1/categories</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Name is required.");

            var entry = new CategoryEntry
            {
                Name = request.Name,
                Description = request.Description ?? string.Empty,
                Attributes = request.Attributes ?? new System.Collections.Generic.Dictionary<string, string>(),
                ParentCategoryIds = request.ParentCategoryIds ?? Array.Empty<int>()
            };

            int id = await _Categories.AddAsync(entry);
            return CreatedAtAction(nameof(GetById), new { id }, new { InstanceId = id });
        }
    }
}
