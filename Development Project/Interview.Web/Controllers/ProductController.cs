using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint;
using Sparcpoint.BL;
using SparcPoint.Inventory.DataModels;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {

        private IBusinessLayer _businessLayer;
        private IDataSerializer _dataDeserializer;
        public ProductController(IBusinessLayer businessLayer,IDataSerializer dataserializer)
        {
            _businessLayer = businessLayer;
            _dataDeserializer = dataserializer;
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            try
            {
                return Task.FromResult((IActionResult)_businessLayer.GetAllProducts());
            }
            catch(Exception ex)
            {
                return Task.FromResult((IActionResult)NotFound());  // Or something else on the lines of exception found
            }
        }


        [HttpPut]
        public Task<IActionResult> AddProduct(string productJson)
        {
            try
            {
                var product = _dataDeserializer.Deserialize<Product>(productJson);

                 _businessLayer.AddProduct(product).GetAwaiter().GetResult();
                return Task.FromResult((IActionResult)Ok());
            }
            catch(Exception ex)
            {
                return Task.FromResult((IActionResult)BadRequest());
            }
        }

        [HttpDelete]
        public Task<IActionResult> RemoveProduct(string productJson)
        {
            try
            {
                var product = _dataDeserializer.Deserialize<Product>(productJson);

                if(_businessLayer.RemoveProduct(product).GetAwaiter().GetResult())
                {
                    return Task.FromResult((IActionResult)Ok());
                }

                return Task.FromResult((IActionResult)BadRequest());
                
            }
            catch (Exception ex)
            {
                return Task.FromResult((IActionResult)BadRequest());
            }
        }

        [HttpGet]
        public Task<IActionResult> FindProduct(string searchString)
        {
            try
            {
                var products = _businessLayer.FindProduct(searchString).Result;
                if(products==null)
                {
                    return Task.FromResult((IActionResult)NotFound());
                }

                return Task.FromResult((IActionResult)Ok(products));
            }
            catch(Exception e)
            {
                return Task.FromResult((IActionResult)BadRequest());
            }
        }

        [HttpGet]
        public Task<IActionResult> NumberOfProducts(string searchparameter)
        {
            try
            {                   
                return Task.FromResult((IActionResult)Ok(_businessLayer.ProductCount(searchparameter).Result));
            }
            catch (Exception e)
            {
                return Task.FromResult((IActionResult)BadRequest());
            }
        }
    }
}
