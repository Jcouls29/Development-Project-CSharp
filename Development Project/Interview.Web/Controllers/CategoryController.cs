using System.Threading;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Repositories;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categories;

        public CategoryController(ICategoryRepository categories)
        {
            _categories = categories;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var id = await _categories.AddAsync(request.ToDomain(), ct);
            return CreatedAtAction(nameof(GetCategory), new { id }, new { instanceId = id });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategory(int id, CancellationToken ct)
        {
            var category = await _categories.GetByIdAsync(id, ct);
            return category == null ? NotFound() : (IActionResult)Ok(category);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListCategories(CancellationToken ct)
        {
            return Ok(await _categories.ListAsync(ct));
        }
    }
}
