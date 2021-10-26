using Interview.Data.ViewModels;
using Interview.Web.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web.Http;

namespace Interview.Web.Controllers
{
    [RoutePrefix("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IGenericRepository<ProductViewModel> _genericRepository;

        public ProductController(IGenericRepository<ProductViewModel> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _genericRepository.GetAll();
            return await Task.FromResult((IActionResult)Ok(products));
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("addProduct")]
        public async Task<IActionResult> AddProduct([System.Web.Http.FromBody] ProductViewModel productViewModel)
        {
            var records = await _genericRepository.Add(productViewModel);
            return await Task.FromResult((IActionResult)Ok());
        }
    }
}
