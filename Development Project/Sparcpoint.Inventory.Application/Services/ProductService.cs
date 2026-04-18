using Interview.Web.Dtos.Requests;
using Sparcpoint.Inventory.Application.Dtos.Requests;
using Sparcpoint.Inventory.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> CreateAsync(CreateProductRequest request)
        {
            // 1. Insert Product
            var productId = await _repo.InsertProductAsync(
                request.Name,
                request.Description,
                string.Join(",", request.ProductImageUris),
                string.Join(",", request.ValidSkus)
            );

            // 2. Insert Attributes
            if (request.Attributes != null)
            {
                foreach (var attr in request.Attributes)
                {
                    await _repo.InsertAttributeAsync(
                        productId,
                        attr.Key,
                        attr.Value
                    );
                }
            }

            // 3. Insert Categories
            if (request.CategoryIds != null)
            {
                foreach (var categoryId in request.CategoryIds)
                {
                    await _repo.InsertProductCategoryAsync(productId, categoryId);
                }
            }

            return productId;
        }

        public async Task<List<int>> SearchAsync(SearchProductsRequest request)
        {
            return await _repo.SearchProductIdsAsync(request.Attributes, request.CategoryIds);
        }
    }
}
