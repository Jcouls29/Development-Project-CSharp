using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Services;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var all = await _service.GetAllAsync();
            return Ok(all);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var p = await _service.GetByIdAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (product == null) return BadRequest();

            var created = await _service.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Search by metadata (match all provided key/value pairs)
        [HttpPost("search")]
        public async Task<IActionResult> SearchByMetadata([FromBody] Dictionary<string, string> metadata)
        {
            var results = await _service.SearchByMetadataAsync(metadata ?? new Dictionary<string, string>());
            return Ok(results);
        }
    }
}
