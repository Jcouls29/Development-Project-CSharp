using Sparcpoint.API.Models;

using Dapper;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using System.Text.Json.Nodes;

namespace Sparcpoint.API.API
{
    public class Products
    {
        private readonly string _connectionString;

        public Products(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<ProductModel>> GetProducts()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT * FROM Products";
                    var products = await connection.QueryAsync<ProductModel>(sql);
                    return products.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Enumerable.Empty<ProductModel>();
            }
        }

        public async Task<ProductModel> AddProduct(ProductModel productModel, ProductAttributesModel productAttributesModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var productSql = "INSERT INTO Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp";

                    productModel.Id = await connection.ExecuteAsync(productSql, productModel);

                    productAttributesModel.Key = productModel.Id;
                    var productAttributesSql = "INSERT INTO Products (Key, Value,) VALUES (@Key, @Value";

                    productAttributesModel.InstanceId = await connection.ExecuteAsync(productAttributesSql, productAttributesModel);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return productModel;
        }

        public async Task<ReturnModel> SearchProducts(SearchModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT p.Id 
                    FROM [dbo].[Product] p 
                    Join [dbo].[ProductAttributes] pa on pa.Key = p.Id 
                    WHERE p.Description Like %@Query% or p.Name Like %@Name% or p.CreatedTimestamp = @CreatedTimestamp or pa.value Like %Query%";


                var run = await connection.QueryAsync(sql, model);

                List<int> productIds = new List<int>();

                foreach (var item in run)
                {
                    productIds.Add(item.Id);
                }

                var searchResults = new ReturnModel
                {
                    ProductIds = productIds
                };

                return searchResults;

            }
        }
    }
}
