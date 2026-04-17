using Microsoft.AspNetCore.Mvc;
using Sparcpoint.SqlServer.Abstractions;
using Sparcpoint.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ISqlExecutor _sqlExecutor;

        private readonly Sparcpoint.Application.Repositories.IProductRepository _productRepository;

        public ProductsController(ISqlExecutor sqlExecutor = null, Sparcpoint.Application.Repositories.IProductRepository productRepository = null)
        {
            _sqlExecutor = sqlExecutor;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            // Delegate to Search with no filters to return all products
            if (_productRepository == null)
                return Ok(new List<ProductResponseDto>());

            var results = await _productRepository.SearchAsync(null, null, null);
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            if (_productRepository != null)
            {
                var id = await _productRepository.CreateProductAsync(dto);
                var response = new ProductResponseDto { Id = id, Name = dto.Name };
                return CreatedAtAction(nameof(GetById), new { id = id }, response);
            }

            var fallback = new ProductResponseDto { Id = 0, Name = dto.Name };
            return CreatedAtAction(nameof(GetById), new { id = 0 }, fallback);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var response = new ProductResponseDto { Id = id, Name = "(not persisted in this demo)" };
            return Ok(response);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] ProductSearchDto dto)
        {
            if (dto == null)
                return BadRequest();

            if (_productRepository == null)
                return StatusCode(500, "Product repository not configured.");

            var results = await _productRepository.SearchAsync(dto.Name, dto.Metadata, dto.Categories);
            return Ok(results);
        }
    }
}
