using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using Sparcpoint.Products.Domain;
using System.Threading.Tasks;

// EVAL: While the norm is to put data access classes in their own project,
// I find the higher cohesiveness of keeping the domain orchestrator/manager in the same project as data access a bigger benefit
namespace Sparcpoint.Products.Data
{
    internal class ProductDataManager
    {
        private static string _connectionString { get; set; }
        public ProductDataManager(string connString)
        {
            _connectionString = connString;
        }

        internal async Task<Product> QueryProductById(int productId)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "@ProductId", productId }
            };
            var parameters = new DynamicParameters(dictionary);
            string sql = "select " +
                "p.ProductId, Manufacturer, ModelName, Description " +
                "from instances.Product p " +
                "where productId = @ProductId";
            using (var connection = new SqlConnection(_connectionString))
            {
                Product product = await connection.QuerySingleAsync<Product>(sql, parameters);
                return product;
            }
        }

        internal async Task<Product> QueryProductWithInventoryItems(int productId)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "@ProductId", productId }
            };
            var parameters = new DynamicParameters(dictionary);
            string sql = "select " +
                "p.ProductId, Manufacturer, ModelName, Description, sku, AttributesJson, QuantityOnHand " +
                "from instances.Product p " +
                "inner join instances.InventoryItem ii on p.productId = ii.ProductId" +
                "where productId = @ProductId";
            using (var connection = new SqlConnection(_connectionString))
            {
                var product = await connection.QuerySingleAsync<Product>(sql, parameters);
                return product;
            }
        }

        internal async Task<IEnumerable<Product>> QueryProductsInInventory()
        {
            string sql = "select ProductId, Manufacturer, ModelName, Description from instances.Product";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Product>(sql);
            }
        }

        internal async Task<IEnumerable<Product>> QueryProductsWithInventoryItems()
        {
            string sql = "select " +
                "p.ProductId, Manufacturer, ModelName, Description, sku, AttributesJson, QuantityOnHand " +
                "from instances.Product p " +
                "inner join instances.InventoryItem ii on p.productId = ii.ProductId";

            using (var connection = new SqlConnection(_connectionString))
            {
                var products = await connection.QueryAsync<Product,InventoryItem,Product>(
                    sql, 
                    (Product, InventoryItem) =>
                    {
                        Product.Items.Add(InventoryItem);
                        return Product;
                    },
                    splitOn: "sku"
                    );
                return products;
            }
        }

        internal async Task AddNewProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Instances.Product (Manufacturer, ModelName, Description) VALUES (@Manufacturer, @ModelName, @Description)";
                var rowsAffected = await connection.ExecuteAsync(sql, product);
            }
        }

        internal async Task UpdateProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Instances.Product " +
                    "SET Manufacturer = @Manufacturer, " +
                    "ModelName = @ModelName, " +
                    "Description = @Description " +
                    "WHERE ProductId = @ProductId";
                var rowsAffected = await connection.ExecuteAsync(sql, product);
            }
        }
    }
}
