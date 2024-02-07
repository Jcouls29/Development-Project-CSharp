﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Z.Dapper.Plus;

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

        internal async Task<Product> QueryProductWithInventoryItems(int productId)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "@ProductId", productId }
            };
            var parameters = new DynamicParameters(dictionary);
            string sql = "select " +
                "p.ProductId, Manufacturer, ModelName, Description, CategoriesJson, sku, AttributesJson, QuantityOnHand " +
                "from instances.Product p " +
                "inner join instances.InventoryItem ii on p.productId = ii.ProductId " +
                "where p.productId = @ProductId";
            using (var connection = new SqlConnection(_connectionString))
            {
                var product = await connection.QueryAsync<Product, InventoryItem, Product>(
                    sql,
                    (p, i) =>
                    {
                        p.Items.Add(i);
                        return p;
                    },
                    parameters,
                    splitOn: "sku"
                    );
                return product.Single<Product>();
            }
        }

        internal async Task<IEnumerable<Product>> QueryProductsWithInventoryItems()
        {
            string sql = "select " +
                "p.ProductId, Manufacturer, ModelName, Description, CategoriesJson, sku, AttributesJson, QuantityOnHand " +
                "from instances.Product p " +
                "inner join instances.InventoryItem ii on p.productId = ii.ProductId";

            using (var connection = new SqlConnection(_connectionString))
            {
                var products = await connection.QueryAsync<Product, InventoryItem, Product>(
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
                var dictionary = new Dictionary<string, object>
                {
                    { "@Manufacturer", product.Manufacturer },
                    { "@ModelName", product.ModelName },
                    { "@Description", product.Description },
                    { "@CategoriesJson", product.CategoriesJson }
                };
                var parameters = new DynamicParameters(dictionary);
                var sql = "INSERT INTO Instances.Product (Manufacturer, ModelName, Description, CategoriesJson) VALUES (@Manufacturer, @ModelName, @Description, @CategoriesJson)";

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            }
        }

        internal async Task UpdateProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "@ProductId", product.ProductId },
                    { "@Manufacturer", product.Manufacturer },
                    { "@ModelName", product.ModelName },
                    { "@Description", product.Description },
                    { "@CategoriesJson", product.CategoriesJson }
                };
                var parameters = new DynamicParameters(dictionary);

                var sql = "UPDATE Instances.Product " +
                    "SET Manufacturer = @Manufacturer, " +
                    "ModelName = @ModelName, " +
                    "Description = @Description, " +
                    "CategoriesJson = @CategoriesJson " +
                    "WHERE ProductId = @ProductId";

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            }
        }

        internal async Task<IEnumerable<Product>> SearchForProducts(ProductSearch product)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "@Manufacturer", product.Manufacturer },
                { "@ModelName", product.ModelName },
                { "@Description", product.Description },
                { "@Category", product.Category },
                { "@Attribute", product.Attribute }
            };
            var parameters = new DynamicParameters(dictionary);
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = product.CreateQueryString();
                var products = await connection.QueryAsync<Product, InventoryItem, Product>(
                  sql,
                  (Product, InventoryItem) =>
                  {
                      Product.Items.Add(InventoryItem);
                      return Product;
                  },
                  parameters,
                  splitOn: "sku"
                  );
                return products;
            }
        }

        internal async virtual Task<int> InventoryCountBySku(string sku)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "@Sku", sku }
            };
            var parameters = new DynamicParameters(dictionary);
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select QuantityOnHand from Instances.InventoryItem where sku = @Sku";
                var quantityOnHand = await connection.QuerySingleAsync<int>(sql, parameters);
                return quantityOnHand;
            }
        }

        internal async Task AddInventoryItem(InventoryItem inventoryItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "@ProductId", inventoryItem.ProductId },
                    { "@Sku", inventoryItem.Sku },
                    { "@AttributesJson", inventoryItem.AttributesJson },
                    { "@QuantityOnHand", inventoryItem.QuantityOnHand }
                };
                var parameters = new DynamicParameters(dictionary);
                var sql = "INSERT INTO Instances.InventoryItem (ProductId, Sku, AttributesJson, QuantityOnHand) VALUES (@ProductId, @Sku, @AttributesJson, @QuantityOnHand)";

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            }
        }

        internal async Task UpdateInventoryItem(InventoryItem inventoryItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var dictionary = new Dictionary<string, object>
                {
                    { "@Sku", inventoryItem.Sku },
                    { "@ProductId", inventoryItem.ProductId },
                    { "@AttributesJson", inventoryItem.AttributesJson },
                    { "@QuantityOnHand", inventoryItem.QuantityOnHand }
                };
                var parameters = new DynamicParameters(dictionary);
                var sql = "UPDATE Instances.InventoryItem " +
                    "SET ProductId = @ProductId, " +
                    "AttributesJson = @AttributesJson, " +
                    "QuantityOnHand = @QuantityOnHand " +
                    "WHERE Sku = @Sku";

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
