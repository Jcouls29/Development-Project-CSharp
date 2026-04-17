using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Interfaces;
using Sparcpoint.Models;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// Controller responsible for managing categories.
    /// Provides operations to retrieve and create categories.
    /// </summary>
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryRepository">Category repository dependency.</param>
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Retrieves all available categories.
        /// </summary>
        /// <returns>A list of categories.</returns>
        /// <response code="200">Returns the list of categories</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Retrieves a category by its identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the category.</param>
        /// <returns>The requested category if found; otherwise, a not found response.</returns>
        /// <response code="200">Category found</response>
        /// <response code="404">Category not found</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound($"Category with Id {id} not found");

            return Ok(category);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="category">Category data to be created.</param>
        /// <returns>The identifier of the newly created category.</returns>
        /// <remarks>
        /// The category name is required.
        /// </remarks>
        /// <response code="201">Category successfully created</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            // EVAL: Validación básica
            if (string.IsNullOrEmpty(category.Name))
                return BadRequest("Category name is required");

            var instanceId = await _categoryRepository.CreateAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = instanceId }, instanceId);
        }
    }
}