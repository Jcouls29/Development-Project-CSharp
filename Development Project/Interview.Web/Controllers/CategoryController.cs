using Interview.Web.Contracts;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CategoryRequest request, CancellationToken cancellationToken)
        {
            if (request is null) return BadRequest();
            var id = await _service.CreateAsync(request.ToModel(), cancellationToken);
            return Created($"/api/v1/categories/{id}", new { id });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _service.GetAsync(id, cancellationToken);
            return category is null ? NotFound() : Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> ListAsync(CancellationToken cancellationToken)
            => Ok(await _service.ListAsync(cancellationToken));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
        {
            if (request is null) return BadRequest();
            var updated = await _service.UpdateAsync(request.ToModel(id), cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var removed = await _service.DeleteAsync(id, cancellationToken);
            return removed ? NoContent() : NotFound();
        }

        [HttpGet("{id:int}/descendants")]
        public async Task<IActionResult> DescendantsAsync(int id, CancellationToken cancellationToken)
            => Ok(await _service.GetDescendantsAsync(id, cancellationToken));
    }
}
