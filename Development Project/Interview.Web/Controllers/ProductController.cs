using Dapper;
using DataService.DataAccess;
using DataService.Interfaces;
using DataService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{

    public class ProductController : Controller
    {

        private readonly IProductData _productData;
        private IEnumerable<ProductsDTO> productsDTOs { get; set; }

        // Constructor injection
        public ProductController(IProductData productData)
        {
            _productData = productData;
        }


        /// <summary>
        /// Retrieves the product by the client and the productname, this is easliy changeable and can be reconfigured to
        /// include the object and all various properties to search on SKU, Quantity and many other items.
        /// this can also be a post and will return JSON, very flexible
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="productName"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/v1/products/{clientId}/{productName}")]
        public async Task<ActionResult<IEnumerable<ProductsDTO>>> GetProducts(int clientId, string productName)
        {
            try
            {
                IEnumerable<ProductsDTO> products = await _productData.GetProducts(clientId, productName);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// This will insert a new inventory transaction
        /// </summary>
        /// <param name="productsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/products")]
        public async Task<ActionResult<IEnumerable<ProductsDTO>>> InsertInventoryTransactions(ProductsDTO productsDTO)
        {
            try
            {
                IEnumerable<ProductsDTO> productTransactions = await _productData.InsertInventoryTransactions(productsDTO);
                return Ok(productTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Removes a Product from the Inventory Transactions
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/products/{transactionId}")]
        public async Task<ActionResult<IEnumerable<ProductsDTO>>> RemoveInventoryTransactions(int transactionId)
        {
            try
            {
                IEnumerable<ProductsDTO> productTransactions = await _productData.RemoveInventoryTransactions(transactionId);
                return Ok(productTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Gets Count, not passing int ClientID but can if needed
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/products/{instanceId}")]
        public async Task<ActionResult<int>> GetProductCount(int instanceId)
        {
            try
            {


                int count = await _productData.GetProductCount(instanceId);

                return Ok(count);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }




}

