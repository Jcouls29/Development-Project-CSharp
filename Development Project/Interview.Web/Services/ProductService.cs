using Interview.Web.DTO;
using Interview.Web.Enums;
using Interview.Web.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sparkpoint.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryContext _context;
        public ProductService(InventoryContext context)
        {
            _context = context;
        }
        public async Task<ProductCreationResponseDto> CreateProductAsync(ProductCreationRequestDto productDto)
        {
            // prepare default result. 
            // it will change if something went wrong.
            var response = new ProductCreationResponseDto()
            {
                Success = true,
                Message = "Product Created"
            };
            // Create the product
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                ValidSkus = productDto.ValidSkus,
                ProductImageUris = productDto.ProductImageUris
            };

            // Add the product to the context
            _context.Products.Add(product);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle any database-related exceptions
                // Log the exception, return appropriate error response, etc.
                response.Success = false;
                response.Message = ex.Message;
            }

            // Get the ID of the newly created product
            var productId = product.InstanceId;
            // some edge cases. 
            // Retrieve the product from the database using its ID
            var createdProduct = await _context.Products.FindAsync(productId);

            if (createdProduct == null)
            {
                // Handle the case where the product was not found
            }

            // Link the product to categories
            foreach (var categoryDto in productDto.Categories)
            {
                // Check if the category exists in the database
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower());

                // If the category doesn't exist, create it
                if (category == null)
                {
                    category = new Category { Name = categoryDto.Name, Description = categoryDto.Description};
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync(); // Save changes to generate category ID
                }

                // Link the product to the category
                var productCategory = new ProductCategory { InstanceId = productId, CategoryInstanceId = category.InstanceId };
                _context.ProductCategories.Add(productCategory);
            }
            // Save changes to link product to categories
            await _context.SaveChangesAsync();

            // Save attributes for the product
            foreach (var attributeDto in productDto.Attributes)
            {
                // Check if the attribute key matches any allowed attribute in the enum
                if (Enum.TryParse<AttributeTypesEnum>(attributeDto.Key, true, out var attributeKey))
                {
                    // Attribute key is valid, save the attribute
                    var productAttribute = new ProductAttribute
                    {
                        InstanceId = productId,
                        Key = attributeDto.Key,
                        Value = attributeDto.Value
                    };
                    _context.ProductAttributes.Add(productAttribute);
                }
                else
                {
                    // Attribute key is not valid, return error response
                    // Ideally, user will use dropdown from UI to choose allowed attributes.
                    // this check just prevents QA team members who use endpoints with json payload. 
                }
            }
            // Record the transaction
            var transaction = new InventoryTransaction
            {
                ProductInstanceId = productId,
                Quantity = 1, // Assuming a new product is added with a quantity of 1
                TypeCategory = TransactionTypesEnum.Create.ToString(),
            };
            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            // Save changes to the database
            await _context.SaveChangesAsync();
            response.Product = product;
            // Return the created product
            return response;
        }

        public Product DeleteProduct(int id)
        {
            // maybe needed in future for hard delete. 
            throw new System.NotImplementedException();
        }
        public async Task<bool> SoftDeleteProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return false; // Product not found
            }

            product.IsActive = false;

            try
            {
                // Save changes to mark product as soft deleted
                await _context.SaveChangesAsync();

                // Record soft delete operation in transaction table
                var transaction = new InventoryTransaction
                {
                    ProductInstanceId = productId,
                    Quantity = -1, // Indicate deletion with a negative quantity
                    TypeCategory = TransactionTypesEnum.Deletion.ToString(),
                };
                _context.InventoryTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                return true; // Soft delete successful
            }
            catch (DbUpdateException)
            {
                // Handle database-related errors
                return false; // Soft delete failed
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var products = _context.Products
                .Include(x => x.ProductAttributes)
                .Include(x => x.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .ToList();
            return products;
        }
        public Product GetProductById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Product UpdateProduct(Product productId)
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<Product> SearchProducts(SearchCriteriaDto criteria)
        {
            // Start with a query for all products
            IQueryable<Product> query = _context.Products
                .Include(x => x.ProductAttributes)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category);

            // Apply filtering based on search criteria
            // search by criteria
            if (!string.IsNullOrEmpty(criteria.Category))
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.Name.ToLower() == criteria.Category.ToLower()));
            }

            // search by name
            if (!string.IsNullOrEmpty(criteria.Name))
            {
                query = query.Where(p => p.Name.Contains(criteria.Name));
            }
            // search by description
            if (!string.IsNullOrEmpty(criteria.Description))
            {
                query = query.Where(p => p.Description.ToLower().Contains(criteria.Description.ToLower()));
            }
            // search by product attributes
            // search attributes by their key and value. both key and value should be provided. 
            criteria.GeneralDetails?.ForEach(generalDetail =>
            {
                if (!string.IsNullOrEmpty(generalDetail.Key) && !string.IsNullOrEmpty(generalDetail.Value))
                {
                    query = query
                        .Where(p => p.ProductAttributes.Any(pa => pa.Key == generalDetail.Key && pa.Value == generalDetail.Value));
                }
            });

            // Execute the query and return the results
            return query.ToList();
        }

        public int GetInventoryCountByMetadata(SearchCriteriaDto criteria)
        {
            // Start with a query for all products
            IQueryable<Product> query = _context.Products
                .Where(t => t.IsActive == true);
            // Apply filtering based on search criteria
            // search by criteria
            if (!string.IsNullOrEmpty(criteria.Category))
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.Name.ToLower() == criteria.Category.ToLower()));
            }

            // search by name
            if (!string.IsNullOrEmpty(criteria.Name))
            {
                query = query.Where(p => p.Name.Contains(criteria.Name));
            }
            // search by description
            if (!string.IsNullOrEmpty(criteria.Description))
            {
                query = query.Where(p => p.Description.ToLower().Contains(criteria.Description.ToLower()));
            }
            // search by product attributes
            // search attributes by their key and value. both key and value should be provided. 
            criteria.GeneralDetails?.ForEach(generalDetail =>
            {
                if (!string.IsNullOrEmpty(generalDetail.Key) && !string.IsNullOrEmpty(generalDetail.Value))
                {
                    query = query
                        .Where(p => p.ProductAttributes.Any(pa => pa.Key == generalDetail.Key && pa.Value == generalDetail.Value));
                }
            });

            // Execute the query and return the results
            return query.ToList().Count();
        }
    }
}
