using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using Sparcpoint.Abstract;
using Sparcpoint.Models.Requests;
using Sparcpoint.Models.Tables;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations
{
    public class ProductService : IProductService
    {
        private readonly string _ConnectionString;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly string ProductsTable = "[Instances].[Products]";
        private readonly string ProductKey = "InstanceId";
        //EVAL: should either extend _sqlExecutor or add additional sql Query service instead of querying directly within the product service

        public ProductService(IOptions<SqlServerOptions> sqlServerOptions, ISqlExecutor sqlExecutor)
        {
            _ConnectionString = sqlServerOptions.Value.ConnectionString ?? throw new ArgumentNullException(nameof(sqlServerOptions.Value.ConnectionString));
            _sqlExecutor = sqlExecutor;
        }

        /// <summary>
        /// Adds a product, product attributes, and product categories to the db
        /// </summary>
        /// <param name="product"></param>
        /// <returns>product id (InstanceId)</returns>
        public async Task<int> AddProductAsync(Product product)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                product.CreatedTimestamp = DateTime.Now;
                var instanceId = await connection.InsertAsync(product, transaction);

                if (product.Categories.Any())
                {
                    foreach(ProductCategory category in product.Categories)
                    {
                        category.InstanceId = instanceId;
                        await connection.InsertAsync(category, transaction);
                    }
                }
                if (product.Attributes.Any())
                {
                    foreach (ProductAttribute attribute in product.Attributes)
                    {
                        attribute.InstanceId = instanceId;
                        await connection.InsertAsync(attribute, transaction);
                    }
                }
                return instanceId;
            });
        }

        /// <summary>
        /// Retrieves a product by the product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<Product> GetProductAsync(int productId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_ConnectionString))
            {
                dbConnection.Open();
                var result = await dbConnection.QueryFirstOrDefaultAsync<Product>($"SELECT * FROM {ProductsTable} p WHERE p.{ProductKey} = {productId}");
                return result;
            }
        }

        /// <summary>
        /// Retrieves product info from the db
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            //EVAL: Need to add in functionality to map attributes and categories into the product object, or create a base product object
            //without the additional properties
            using (IDbConnection dbConnection = new SqlConnection(_ConnectionString))
            {
                dbConnection.Open();
                var results = await dbConnection.QueryAsync<Product>($"SELECT * FROM {ProductsTable}");
                return results;
            }
        }
        public async Task<IEnumerable<Product>> GetProductsAsync(ProductRequest request)
        {
            throw new NotImplementedException();
        }
    }
}