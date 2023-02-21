using AutoMapper;
using Sparcpoint.Entities;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public interface IProductService
    {
        Task<Product> AddProduct(ProductModel productModel);
        Task<Product> DeleteProduct(Guid productId);
        Task<bool> ProductExists(Guid productId);
        Task<Product> UpdateProduct(ProductModel productModel);
    }

    public class ProductService : IProductService
    {
        public readonly IMapper _mapper;
        public ProductService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<Product> AddProduct(ProductModel productModel)
        {
           var addedProduct = await Task.Run(() =>
            {
                var product = _mapper.Map<Product>(productModel);
                //_db.Product.Add(product);
                //await _db.SaveChangesAsync();
                return product;
            });

            return addedProduct;
        }

        public async Task<Product> UpdateProduct(ProductModel productModel)
        {
            var addedProduct = await Task.Run(() =>
            {
                //Get existing Product
                //var existingProduct = await _db.Product.FindAsync(productModel.ProductId);
                //EVAL: Returns this fancy thing:
                var existingProduct = new Product()
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Existing Product",
                    Description = "I'm a description",
                    Categories = new Dictionary<string, int>() { { "Blue", 2 }, { "Sale", 1 } },
                    MetaData = new Dictionary<string, object>() { { "Sku", 19932 }, { "Color", "Blue" } },
                    IsDeleted = false
                };
                //EVAL: Mapper automatically applies changes to existing entity and flags EF that its been modified.
                _mapper.Map(productModel, existingProduct);
                //EVAL: Then we save
                //await _db.SaveChangesAsync();
                return existingProduct;
            });

            return addedProduct;

        }
        public async Task<bool> ProductExists(Guid productId)
        {
            //call DB server to check existence
            //var doesExist = _db.Product.Any(x => x.ProductId == product.ProductId);
            var doesExist = true;
            return await Task.FromResult(doesExist);
        }

        public async Task<Product> DeleteProduct(Guid productId)
        {
            var existingProduct = await Task.Run(() =>
            {
                //Get existing Product
                //var existingProduct = await _db.Product.FindAsync(productId);
                //EVAL: Returns this fancy thing:
                return new Product()
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Existing Product",
                    Description = "I'm a description",
                    Categories = new Dictionary<string, int>() { { "Blue", 2 }, { "Sale", 1 } },
                    MetaData = new Dictionary<string, object>() { { "Sku", 19932 }, { "Color", "Blue" } },
                    IsDeleted = false
                };
            });

            existingProduct.IsDeleted = true;
            //EVAL: Then we save
            //await _db.SaveChangesAsync();
            return existingProduct;

        }
    }
}
