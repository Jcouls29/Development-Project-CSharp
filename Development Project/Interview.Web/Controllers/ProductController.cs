using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.Models;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductManagementService _productManagementService;

        /// <summary>
        /// The constructor for the ProductController is used to inject the IProductManagementService
        /// </summary>
        /// <param name="productManagementService"></param>
        public ProductController(IProductManagementService productManagementService)
        {
            _productManagementService = productManagementService;
        }

        /// <summary>
        /// This method is used to return all products
        /// </summary>
        /// <remarks>All crud operations are handled by the productManagementService</remarks>
         public async Task<IActionResult> Products()
        {
            var products = await _productManagementService.GetAllProductsAsync();
            return View(products);
        }

        /// <summary>
        /// Get a single product and its attributes by name
        /// </summary>
        /// <param name="name"></param>
        [HttpGet("{name}")]
        public async Task<Product> GetProduct(string name)
        {
            return await _productManagementService.GetProductAsync(name);
        }

        /// <summary>
        /// Add a product to the database
        /// </summary>
        /// <param name="product"></param>
        [HttpPost]
        public async Task<IActionResult> AddProductAsync([FromBody] Product product)
        {
            // EVAL: Add product and return 201 Created
            await _productManagementService.AddProductAsync(product);
            return CreatedAtAction(nameof(AddProductAsync), new { product.Name }, product);
        }
    }
}
