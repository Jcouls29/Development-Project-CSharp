using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Interfaces;
using Sparcpoint.Models.Requests;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// Controller responsible for managing products.
    /// Provides operations to search, retrieve, and create products.
    /// </summary>
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productRepository">Product repository dependency.</param>
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Searches for products based on the specified criteria.
        /// </summary>
        /// <param name="request">Search parameters for filtering products.</param>
        /// <returns>A list of products matching the search criteria.</returns>
        /// <response code="200">Returns the list of products</response>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request)
        {
            var products = await _productRepository.SearchAsync(request);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a product by its identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the product.</param>
        /// <returns>The requested product if found; otherwise, a not found response.</returns>
        /// <response code="200">Product found</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound($"Product with Id {id} not found");

            return Ok(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="request">Data required to create the product.</param>
        /// <returns>The identifier of the newly created product.</returns>
        /// <remarks>
        /// Both product name and description are required.
        /// </remarks>
        /// <response code="201">Product successfully created</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            // EVAL: Validación básica antes de insertar
            if (string.IsNullOrEmpty(request.Name))
                return BadRequest("Product name is required");

            if (string.IsNullOrEmpty(request.Description))
                return BadRequest("Product description is required");

            var instanceId = await _productRepository.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = instanceId }, instanceId);
        }
    }
}