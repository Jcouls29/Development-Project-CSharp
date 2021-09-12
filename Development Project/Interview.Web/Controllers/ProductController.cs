using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.models;

namespace Interview.Web.Controllers
{
    
    public class ProductController : Controller
    {
        IProduct iproductdetails;
        public ProductController(IProduct _iproductdetails)
        {
            iproductdetails = _iproductdetails;
        }
        /// <summary>
        // to get product list
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/products/getproducts")]
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            IEnumerable<ProductDetailsModel> prodDetails = iproductdetails.GetAllProducts();
            return Task.FromResult((IActionResult)Ok(prodDetails));
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
            return "value";
        }
        /// <summary>
        /// Add products
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("api/v1/products/addproducts")]
        [HttpPost]
        public Task<IActionResult> AddProduct([FromBody] ProductDetailsModel data)
        {
         iproductdetails.AddProduct(data);
            return Task.FromResult((IActionResult)Ok());
        }
    }
}
