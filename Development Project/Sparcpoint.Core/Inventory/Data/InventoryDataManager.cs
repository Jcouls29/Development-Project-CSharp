using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using Sparcpoint.Product.Domain;
using System.Threading.Tasks;

// EVAL: While the norm is to put data access classes in their own project,
// I find the higher cohesiveness of keeping the domain orchestrator/manager in the same project as data access a bigger benefit
namespace Sparcpoint.Inventory.Data
{
    internal class InventoryDataManager
    {
        private static string _connectionString { get; set; }
        public InventoryDataManager(string connString)
        {
            _connectionString = connString;
        }

        public async Task<ProductItem> QueryProduct(int productId)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "@ProductId", productId }
            };
            var parameters = new DynamicParameters(dictionary);
            string sql = "select " +
                "p.ProductId, Manufacturer, BrandName, Description, sku, AttributesJson, QuantityOnHand " +
                "from instances.Product p " +
                "where productId = @ProductId";
            using (var connection = new SqlConnection(_connectionString))
            {
                ProductItem product = await connection.QuerySingleAsync<ProductItem>(sql, parameters);
                return product;
            }
        }

        public async Task<ProductItem> QueryProductWithInventoryItems(int productId)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "@ProductId", productId }
            };
            var parameters = new DynamicParameters(dictionary);
            string sql = "select " +
                "p.ProductId, Manufacturer, BrandName, Description, sku, AttributesJson, QuantityOnHand " +
                "from instances.Product p " +
                "inner join instances.InventoryItem ii on p.productId = ii.ProductId" +
                "where productId = @ProductId";
            using (var connection = new SqlConnection(_connectionString))
            {
                var product = await connection.QuerySingleAsync<ProductItem>(sql, parameters);
                return product;
            }
        }

        public async Task<IEnumerable<ProductItem>> QueryProductsInInventory()
        {
            string sql = "select ProductId, Manufacturer, BrandName, Description from instances.Product";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<ProductItem>(sql);
            }
        }

        public async Task<IEnumerable<ProductItem>> QueryProductsWithInventoryItems()
        {
            string sql = "select " +
                "p.ProductId, Manufacturer, BrandName, Description, sku, AttributesJson, QuantityOnHand " +
                "from instances.Product p " +
                "inner join instances.InventoryItem ii on p.productId = ii.ProductId";

            using (var connection = new SqlConnection(_connectionString))
            {
                var products = await connection.QueryAsync<ProductItem,InventoryItem,ProductItem>(
                    sql, 
                    (ProductItem, InventoryItem) =>
                    {
                        ProductItem.Items.Add(InventoryItem);
                        return ProductItem;
                    },
                    splitOn: "sku"
                    );
                return products;
            }
        }
    }
}
