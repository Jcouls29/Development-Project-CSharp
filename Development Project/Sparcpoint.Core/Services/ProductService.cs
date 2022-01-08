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
            //EVAL: eventually want to use a mapper here but start simple

            var product = new Product()
            {
                Name = req.Name,
                Description = req.Description,
                ProductImageUris = string.Join(",", req.ProductImageUris.ToArray()),
                ValidSkus = string.Join(",", req.ValidSkus.ToArray()),
                CreatedTimestamp = DateTime.UtcNow
            };

            var createdId = await _productDataService.CreateProductAsync(product);

            //EVAL: for efficieny imporvement I'd want to write a method that accepts
            //multiple attributes at once to prevent too many DB connections
            if (req.ProductAttributes != null && req.ProductAttributes.Count > 0)
            {
                foreach (var attr in req.ProductAttributes)
                {
                    await _productDataService.AddAttributeToProduct(createdId, attr);
                }
            }

            //EVAL: for efficieny imporvement I'd want to write a method that accepts
            //multiple attributes at once to prevent too many DB connections
            if (req.CategoryIds != null && req.CategoryIds.Count > 0)
            {
                foreach (var cat in req.CategoryIds)
                {
                    await _productDataService.AddProductToCategory(cat, createdId);
                }
            }
        }

        public async Task<List<Product>> SearchProducts(ProductSearchRequest req)
        {
            //default to product name for order
            //default to asc for sort order
            //default page to 1
            //default page count to 25
            //default search by to name
            return new List<Product>();
        }


        public async Task AddAttributesToProduct(int productId, List<KeyValuePair<string, string>> attributes)
        {
            foreach (var attr in attributes)
            {
                //EVAL: for efficieny imporvement I'd want to write a method that accepts
                //multiple attributes at once to prevent too many DB connections
                await _productDataService.AddAttributeToProduct(productId, attr);
            }
        }

        public async Task AddProductToCategories(int productId, List<int> categories)
        {
            foreach (var cat in categories)
            {
                //EVAL: for efficieny imporvement I'd want to write a method that accepts
                //multiple attributes at once to prevent too many DB connections
                await _productDataService.AddProductToCategory(cat, productId);
            }
        }

        public async Task<List<KeyValuePair<string, string>>> GetAttributesForProduct(int productId)
        {
            return await _productDataService.GetAttributesForProduct(productId);
        }
    }
}
