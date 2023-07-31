using AutoMapper;
using Interview.Web.Services.interfaces;
using Sparcpoint.Contexts;
using Sparcpoint.Entities;
using Sparcpoint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductContext _context;
        public readonly IMapper _mapper;
        public ProductService(ProductContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Product>> GetAllProduct()
        {
            var products = await Task.Run(() =>
            {
                var products = new List<Product>()
                {
                    new Product(){
                        ProductId = Guid.NewGuid(),
                        Name = "Car A",
                        Description = "Red Kia sportage",
                        Categories = new Dictionary<string, int>() { { "Red", 1 }, { "Sale", 1 } },
                        MetaData = new Dictionary<string, object>() { { "Sku", 14593 }, { "Color", "Red" } },
                        IsDeleted = false
                    },

                    new Product(){
                        ProductId = Guid.NewGuid(),
                        Name = "Car B",
                        Description = "Maroon Ford SUV",
                        Categories = new Dictionary<string, int>() { { "Maroon", 2 }, { "Sale", 0 } },
                        MetaData = new Dictionary<string, object>() { { "Sku", 6213 }, { "Color", "Maroon" } },
                        IsDeleted = false
                    },

                    new Product(){
                        ProductId = Guid.NewGuid(),
                        Name = "Car C",
                        Description = "Blue Nissan Altima",
                        Categories = new Dictionary<string, int>() { { "Blue", 3 }, { "Sale", 0 } },
                        MetaData = new Dictionary<string, object>() { { "Sku", 16317 }, { "Color", "Blue" } },
                        IsDeleted = false
                    },

                    new Product(){
                        ProductId = Guid.NewGuid(),
                        Name = "Car D",
                        Description = "Black Minivan",
                        Categories = new Dictionary<string, int>() { { "Black", 3 }, { "Sale", 0 } },
                        MetaData = new Dictionary<string, object>() { { "Sku", 15700 }, { "Color", "Black" } },
                        IsDeleted = false
                    },

                    new Product(){
                        ProductId = Guid.NewGuid(),
                        Name = "Car E",
                        Description = "Brown Station Wagon",
                        Categories = new Dictionary<string, int>() { { "Brown", 5 }, { "Sale", 2 } },
                        MetaData = new Dictionary<string, object>() { { "Sku", 9545 }, { "Color", "Brown" } },
                        IsDeleted = true
                    }
            };
                return products.Where(x => !x.IsDeleted).ToList();
            });

            return products;
        }

        public async Task<Product> AddProduct(ProductModel productModel)
        {
            var addedProduct = await Task.Run(() =>
            {
                var product = _mapper.Map<Product>(productModel);

                return product;
            });

            //_context.Products.Add(product)

            //EVAL: Save changes to database here, still needs to be implemented
            //await _context.SaveChangesAsync();

            return addedProduct;
        }

        public async Task<Product> UpdateProduct(ProductModel productModel)
        {
            var addedProduct = await Task.Run(() =>
            {
                var existingProduct = new Product()
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Car A",
                    Description = "Red Kia sportage",
                    Categories = new Dictionary<string, int>() { { "Red", 2 }, { "Sale", 1 } },
                    MetaData = new Dictionary<string, object>() { { "Sku", 14593 }, { "Color", "Red" } },
                    IsDeleted = false
                };
                _mapper.Map(productModel, existingProduct);

                //_context.Products.Update(product)

                //EVAL: Save changes to database here, still needs to be implemented
                //await _context.SaveChangesAsync();

                return existingProduct;
            });

            return addedProduct;

        }
        public async Task<bool> ProductExists(Guid productId)
        {
            var doesExist = true;
            return await Task.FromResult(doesExist);
        }

        public async Task<Product> DeleteProduct(Guid productId)
        {
            var selectedProduct = await Task.Run(() =>
            {
                return new Product()
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Car A",
                    Description = "Red Kia sportage",
                    Categories = new Dictionary<string, int>() { { "Red", 2 }, { "Sale", 1 } },
                    MetaData = new Dictionary<string, object>() { { "Sku", 14593 }, { "Color", "Red" } },
                    IsDeleted = false
                };
            });

            selectedProduct.IsDeleted = true;

            //EVAL: need to update here

            //EVAL: Save changes to database here, still needs to be implemented
            //await _context.SaveChangesAsync();

            return selectedProduct;

        }
    }
}
