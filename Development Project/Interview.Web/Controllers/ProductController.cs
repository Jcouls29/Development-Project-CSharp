using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Interview.Data.Contracts;
using Interview.Data.Models;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {        
        private readonly IRepository<Product> _repository;

        public ProductController(IRepository<Product> repository)
        {            
            this._repository = repository;
        }

        /// <summary>
        /// action method to get all products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {                                   
            var products = this._repository.GetAll();
                        
            return Task.FromResult((IActionResult)Ok(products));
        }

        /// <summary>
        /// action method to create new product
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>

        [HttpPost]
        public Task<IActionResult> AddNewProduct(Product prod)
        {
            this._repository.Add(prod);

            return Task.FromResult((IActionResult)Ok(prod));
        }
    }
}
