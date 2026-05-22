using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Services.Interfaces;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// API Controller for category management.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new category with optional hierarchical relationships.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = await _categoryService.CreateCategoryAsync(request);
                
                return CreatedAtAction(
                    nameof(GetCategoryById),
                    new { id = category.InstanceId },
                    category);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Category creation failed: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating category");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        /// <summary>
        /// Retrieves a category by ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponse>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            
            if (category == null)
                return NotFound(new { error = $"Category with ID {id} not found" });

            return Ok(category);
        }

        /// <summary>
        /// Gets all categories including hierarchical relationships.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoryResponse>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }
    }
}