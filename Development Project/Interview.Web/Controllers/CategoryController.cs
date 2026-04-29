using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// Manages product categories including hierarchical parent-child relationships.
    /// </summary>
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _Categories;

        public CategoryController(ICategoryRepository categories)
        {
            _Categories = categories ?? throw new ArgumentNullException(nameof(categories));
        }

        /// <summary>
        /// Creates a new category. Optionally nest it under existing parent categories
        /// by supplying ParentCategoryIds. Hierarchy is stored as an adjacency list.
        /// Returns 400 if any ParentCategoryId does not reference an existing category.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequest request)
        {
            var categoryId = await _Categories.AddAsync(request);
            return StatusCode(201, categoryId);
        }

        /// <summary>
        /// Returns all categories. Hierarchy is represented via ParentCategoryIds on each entry;
        /// clients reconstruct the tree from this flat list.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Category>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _Categories.GetAllAsync();
            return Ok(categories);
        }
    }
}
