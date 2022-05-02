using Interview.Web.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class ProductController : Controller
    {

        // NOTE: Sample Action
        //[HttpGet]
        //public Task<IActionResult> GetAllProducts()
        //{
        //    return Task.FromResult((IActionResult)Ok(new object[] { }));
        //}

        [HttpGet]
        [Route("addProduct")]
        public async Task<string> AddProduct(string name = "testname",
            string Description = "testname", string ProductImageUris = "testname", string ValidSkus = "testname")
        {
            /* add Product To DB*/
            Products model = new Products();
            model.Name = name;
            model.Description = Description;
            model.ProductImageUris = ProductImageUris;
            model.ValidSkus = ValidSkus;
            return await Task.Run(() => Database.AddProduct(model));
        }

        [HttpGet]
        [Route("searchProducts")]
        public async Task<string> searchProducts([FromBody] Products Products)
        {
            /* search products via name and Description and return a list of Products*/
            Products model = new Products();
            model.Name = Products.Name;
            model.Description = Products.Description;
            return await Task.Run(() => Database.GetProduct(model));
        }

        [HttpGet]
        [Route("searchProductTags")]
        public async Task<List<Products>> searchProductByTags(List<ProductsAttributes> ProductsAttributes)
        {
            /* send a list of ProductsAttributes and return a list of Products*/
            ProductsAttributes model = new ProductsAttributes();
            var ListOfProductsAttributes = new List<ProductsAttributes>();

            foreach (var item in ProductsAttributes)
            {
                ListOfProductsAttributes.Add(item);
            }

            return await Task.Run(() => Database.searchProductByTags(ListOfProductsAttributes));
        }

        [HttpPost]
        [Route("RemoveProductsfromInventory")]
        public async Task<string> RemoveProductsfromInventory(List<Products> Products)
        {
            Products model = new Products();
            var ListOfProducts = new List<Products>();

            foreach (var item in Products)
            {
                ListOfProducts.Add(item);
            }

            return await Task.Run(() => Database.RemoveProductfromInventory(ListOfProducts));
        }


    }
}
