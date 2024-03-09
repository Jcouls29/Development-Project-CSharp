using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Models;
using Dapper;
using Sparcpoint.SqlServer.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Services
{
    public class ClientsProductService
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ClientsProductService(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        /// <summary>
        /// Retrieves all products along with their corresponding categories.
        /// </summary>
        public async Task<List<Product>> GetAllProductsAndCorrespondingCategory()
        {
            // SQL query to select all products with their corresponding category names
            const string selectAllProductsQuery = @"SELECT p.*, c.Name AS CategoryName FROM inventory.Instances.Products p
                    LEFT JOIN inventory.Instances.ProductCategories pc ON p.InstanceId = pc.InstanceId
                    LEFT JOIN inventory.Instances.Categories c ON c.InstanceId = pc.CategoryInstanceId";
            
            // Execute the query asynchronously using Dapper
            var products = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
            {
                var result = await conn.QueryAsync<Product>(sql: selectAllProductsQuery, transaction: trnx);
                return result;
            });

            // Convert the query result to a list and return
            return products.ToList();
        }

        /// <summary>
        /// Finds products based on the provided search criteria.
        /// </summary>
        public async Task<List<Product>> FindProduct(ProductSearchModel searchModel)
        {

               string query = "SELECT * FROM inventory.Instances.Products WHERE 1=1";

               if (!string.IsNullOrWhiteSpace(searchModel.CategoryName))
               {
                    query += " AND InstanceId IN (SELECT InstanceId FROM inventory.Instances.ProductCategories pc INNER JOIN inventory.Instances.Categories c ON pc.CategoryInstanceId = c.InstanceId WHERE c.Name LIKE @CategoryName)";
               }

               if (!string.IsNullOrWhiteSpace(searchModel.SKU))
               {
                    query += " AND ValidSkus LIKE @SKU";
               }

               if (!string.IsNullOrWhiteSpace(searchModel.Name))
               {
                    query += " AND Name LIKE @Name";
               }

               // Add more conditions for other search criteria as needed

               // Execute the query asynchronously
               var products = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
               {
                    var result = await conn.QueryAsync<Product>(sql: query, param: new
                    {
                         CategoryName = $"%{searchModel.CategoryName}%",
                         SKU = $"%{searchModel.SKU}%",
                         Name = $"%{searchModel.Name}%"
                    }, transaction: trnx);
                    return result;
               });

               // Convert the query result to a list and return
               return products.ToList();
        }

        /// <summary>
        /// Creates a new product with the provided details.
        /// </summary>
        public async Task<Product> CreateProduct(CreateProductDetailsModel productModel)
        {
            // Insert new product details into the database
            const string insertProductQuery = @"INSERT INTO inventory.Instances.Products (Name, Description, ProductImageUris, ValidSkus)
                                               VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                                               SELECT CAST(scope_identity() AS int);";

            int productId = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
            {
                // Execute the insert query asynchronously and retrieve the new product's ID
                int id = await conn.ExecuteScalarAsync<int>(insertProductQuery, new
                {
                    productModel.Name,
                    productModel.Description,
                    productModel.ProductImageUris,
                    productModel.ValidSkus
                }, trnx);
                return id;
            });

            // Check if the product creation was successful
            if (productId <= 0)
                return null; // Return null if product creation fails

            // Retrieve the category ID
            const string getCategoryQuery = "SELECT InstanceId FROM inventory.Instances.Categories WHERE Name = @CategoryName";
            int categoryId = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
            {
                int catId = await conn.ExecuteScalarAsync<int>(getCategoryQuery, new
                {
                    CategoryName = productModel.CategoryName
                }, trnx);
                return catId;
            });

            // Insert into ProductCategories table to associate product with category
            const string insertProductCategoryQuery = @"INSERT INTO inventory.Instances.ProductCategories (InstanceId, CategoryInstanceId)
                                                        VALUES (@ProductId, @CategoryId);
                                                        SELECT CAST(scope_identity() AS int);";

            int productCategoryId = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
            {
                int catId = await conn.ExecuteScalarAsync<int>(insertProductCategoryQuery, new
                {
                    ProductId = productId,
                    CategoryId = categoryId
                }, trnx);
                return catId;
            });

            // Retrieve the created product along with category information
            const string getProductQuery = @"SELECT p.*, c.Name AS CategoryName FROM inventory.Instances.Products p
                                             LEFT JOIN inventory.Instances.ProductCategories pc ON p.InstanceId = pc.InstanceId
                                             LEFT JOIN inventory.Instances.Categories c ON c.InstanceId = pc.CategoryInstanceId
                                             WHERE p.InstanceId = @ProductId;";

            // Execute the query asynchronously to retrieve the product details
            Product newProduct = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
            {
                var product = await conn.QueryFirstOrDefaultAsync<Product>(getProductQuery, new { ProductId = productId }, trnx);
                return product;
            });

            return newProduct;
        }

        /// <summary>
        /// Adds a specified quantity of the product to the inventory.
        /// </summary>
        public async Task AddToInventoryAsync(int productId, int quantity)
        {
            const string addToInventoryQuery = @"UPDATE inventory.Instances.Products 
                                                 SET Quantity = Quantity + @Quantity 
                                                 WHERE InstanceId = @ProductId";

            // Execute the query asynchronously to add the specified quantity to the inventory
            int affectedRows = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
            {
                return await conn.ExecuteAsync(addToInventoryQuery, new { ProductId = productId, Quantity = quantity }, trnx);
            });

            // Return true if any rows were affected, indicating successful addition to inventory
            return affectedRows > 0;
        }

        /// <summary>
        /// Removes a specified quantity of the product from the inventory.
        /// </summary>
        public async Task<bool> RemoveFromInventoryAsync(int productId, int quantity)
        {
            const string removeFromInventoryQuery = @"UPDATE inventory.Instances.Products 
                                                      SET Quantity = Quantity - @Quantity 
                                                      WHERE InstanceId = @ProductId AND Quantity >= @Quantity";

            // Execute the query asynchronously to remove the specified quantity from the inventory
            int affectedRows = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
            {
                return await conn.ExecuteAsync(removeFromInventoryQuery, new { ProductId = productId, Quantity = quantity }, trnx);
            });

            // Return true if any rows were affected, indicating successful removal from inventory
            return affectedRows > 0;
        }

        /// <summary>
        /// Retrieves the count of product inventory based on a unique product identifier or metadata.
        /// </summary>
        /// <param name="identifier">Unique product identifier or metadata.</param>
        /// <returns>The count of product inventory.</returns>
        public async Task<int> GetProductInventoryCountAsync(string identifier)
        {
            const string getProductInventoryCountQuery = @"
                SELECT SUM(Quantity) AS TotalInventoryCount 
                FROM inventory.Instances.Products 
                WHERE UniqueIdentifier = @Identifier OR Metadata = @Identifier";

            // Execute the query asynchronously to retrieve the inventory count
            int inventoryCount = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
            {
                return await conn.ExecuteScalarAsync<int>(getProductInventoryCountQuery, new { Identifier = identifier }, trnx);
            });

            return inventoryCount;
        }
    }
}
