using Sparcpoint.Inventory.Core.Requests;
using Sparcpoint.Inventory.Repository.Interfaces;
using Sparcpoint.InventoryService.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repository.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IProductService _productService;
        public ProductRepository(IProductService productService)
        {
            _productService = productService.ThrowIfNull(nameof(productService));
        }
        
        public async Task<int> AddProductAsync(AddProductRequest addProductRequest)
        {
            var productInstanceId = await _productService.AddProductAsync(addProductRequest);
            
            return productInstanceId;
        }

        public async Task<List<int>> AddProductAttributesAsync(List<AddProductAttributesRequest> addProductAttributesRequests)
        {
            var productAttributesIntanceIds = await _productService.AddProductAttributesAsync(addProductAttributesRequests);

            return productAttributesIntanceIds;
        }
    }
}
