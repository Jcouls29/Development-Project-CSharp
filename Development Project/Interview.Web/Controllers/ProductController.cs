using Interview.Web.models;
using Interview.Web.Services;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        /// <summary>
        /// Contains the service
        /// </summary>
        private readonly IProductService _service;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="service">The service.</param>
        public ProductController(IProductService service)
        {
            _service = service;
        }
        /// <summary>
        // to get product list
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/products/getproducts")]
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            var products = this._service.GetAllProducts();
            return Task.FromResult((IActionResult)Ok(products));
        }

        /// <summary>
        /// To get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/v1/products/getproductsbyid")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var products = this._service.GetProductById(id);
            return Task.FromResult((IActionResult)Ok(products));
        }


        /// <summary>
        /// Add products
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("api/v1/products/addproduct")]
        [HttpPost]
        public Task<IActionResult> AddProduct([FromBody] ProductModel data)
        {
         this._service.AddProduct(data);
            return Task.FromResult((IActionResult)Ok(data));
        }

        /// <summary>
        /// Add products
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("api/v1/products/updateproduct")]
        [HttpPost]
        public Task<IActionResult> UpdateProduct([FromBody] ProductModel data)
        {
            this._service.UpdateProduct(data);
            return Task.FromResult((IActionResult)Ok(data));
        }

        /// <summary>
        /// Add products
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("api/v1/products/deleteproduct")]
        [HttpDelete]
        public Task<IActionResult> DeleteProduct([FromBody] ProductModel data)
        {
            this._service.DeleteProduct(data);
            return Task.FromResult((IActionResult)Ok(data));
        }
    }
}
