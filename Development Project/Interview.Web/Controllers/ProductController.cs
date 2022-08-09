using Interview.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sparcpoint;
using Sparcpoint.BusinessLayer.Product;
using Sparcpoint.Mappers.DomainToEntity;
using Sparcpoint.Models;
using Sparcpoint.Models.DomainDto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    
    public class ProductController : Controller
    {
        private readonly IProductLayer _productManager;
        private readonly IDataSerializer _serialize;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductLayer productManager, IDataSerializer serialize, ILogger<ProductController> logger)
        {
            _productManager = productManager;
            _serialize = serialize;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles ="admin")]
        [ModelValidation]
        public async Task<IActionResult> AddProducts([FromBody] ProductDto product)
        {
            _logger.LogInformation("Add Product Called with Request {0}",_serialize.Serialize<ProductDto>(product));
            var productList = await _productManager.AddProduct(ProductEntityMapper.MapDTOtoDomain(product));
            return (IActionResult)Ok(_serialize.Serialize<Products>(productList));
        }
        [HttpGet]
        [Authorize]
        [ModelValidation]
        public async Task<IActionResult> SearchProducts(string name, string description, string productImageUri, string validSkusGuid, string category)
        {
            FilterParam filterParam = new FilterParam()
            {
                Name = name,
                Description = description,
                ProductImageUri = productImageUri,
                ValidSkus = validSkusGuid,
                Category = category
            };
            _logger.LogInformation("SearchProducts Called with Request {0}", _serialize.Serialize<FilterParam>(filterParam));
            var productList = await _productManager.SearchProduct(filterParam);
            return (IActionResult)Ok(_serialize.Serialize<List<Products>>(productList));
        }

    }
}
