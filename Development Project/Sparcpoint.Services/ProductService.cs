using Sparcpoint.DataRepository.Interfaces;
using Sparcpoint.Entities;
using Sparcpoint.Models.Request.Product;
using Sparcpoint.Models.Response.Product;
using Sparcpoint.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        private readonly ICategoryService _categoryService;

        public ProductService(
            IProductRepository productRepository,
            ICategoryService categoryService)
        {
            this._productRepository = productRepository;
            this._categoryService = categoryService;
        }

        public async Task<GetProductResponse> GetAllProducts(GetProductRequest request)
        {
            var products = await this._productRepository.GetAll();

            var productResponse = new List<ProductResponse>();

            //EVAL: Preparing response like this is not efficient way.
            //We used AutoMapper(https://docs.automapper.org/en/stable/index.html)
            foreach (var product in products)
            {
                productResponse.Add(await this.PrepareProductResponse(product));
            }

            return new GetProductResponse
            {
                Products = productResponse
            };
        }

        public async Task<ProductResponse> GetProductById(GetProductRequestById request)
        {
            var product = await _productRepository.GetById(request.ProductId);

            if (product == null)
            {
                return new ProductResponse();
            }

            return await this.PrepareProductResponse(product);
        }

        private async Task<ProductResponse> PrepareProductResponse(Product product)
        {
            var categoryRequest = new GetProductRequestById(product.InstanceId);

            return new ProductResponse
            {
                ProductId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = product.ProductImageUris,
                ValidSkus = product.ValidSkus,
                CreatedTimestamp = product.CreatedTimestamp,
                Categories = await this._categoryService.GetProductCategories(categoryRequest)
            };
        }

        public async Task<InsertProductResponse> InsertProduct(InsertProductRequest insertProductRequest)
        {
            var productsResponse = new List<ProductResponse>();

            foreach (var product in insertProductRequest.Products)
            {
                // AutoMapper is the best way to make this entity
                var productEntity = CreateProductEntity(product);

                productEntity.InstanceId = await _productRepository.Insert(productEntity);

                var response = await this.PrepareProductResponse(productEntity);

                productsResponse.Add(response);
            }

            return new InsertProductResponse
            {
                Products = productsResponse
            };
        }

        public Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest updateProductRequest)
        {
            throw new NotImplementedException();
        }

        private static Product CreateProductEntity(ProductRequest request)
        {
            return new Product
            {
                Name = request.Name,
                Description = request.Description,
                ProductImageUris = request.ProductImageUris,
                ValidSkus = request.ValidSkus,
                CreatedTimestamp = DateTime.UtcNow // This can be done in Repository level too
            };
        }
    }
}