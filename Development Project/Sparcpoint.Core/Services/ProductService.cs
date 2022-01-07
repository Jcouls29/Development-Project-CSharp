using Sparcpoint.DataServices;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public class ProductService: IProductService
    {
        private IProductDataService _productDataService;

        public ProductService(IProductDataService productDataService)
        {
            _productDataService = productDataService;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _productDataService.GetProducts();
        }

        public async Task CreateProductAsync(CreateProductRequest req)
        {
            //eventually want to use a mapper here but start simple

            var product = new Product()
            {
                Name = req.Name,
                Description = req.Description,
                ProductImageUris = string.Join(",", req.ProductImageUris.ToArray()),
                ValidSkus = string.Join(",", req.ValidSkus.ToArray()),
                CreatedTimestamp = DateTime.UtcNow
            };

            var createdId = await _productDataService.CreateProductAsync(product);

            if (req.ProductAttributes != null && req.ProductAttributes.Count > 0)
            {
                foreach (var attr in req.ProductAttributes)
                {
                    await _productDataService.AddAttributeToProduct(createdId, attr);
                }
            }

            if (req.CategoryIds != null && req.CategoryIds.Count > 0)
            {
                foreach (var cat in req.CategoryIds)
                {
                    await _productDataService.AddProductToCategory(createdId, cat);
                }
            }
        }
    }
}
