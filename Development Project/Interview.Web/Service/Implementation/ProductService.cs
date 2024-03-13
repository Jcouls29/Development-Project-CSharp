using Interview.Web.Models;
using Interview.Web.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Sparcpoint.SqlServer.Abstractions;
using Dapper;
using System.Linq;
using Dapper.Contrib.Extensions;

namespace Interview.Web.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly string TableName = "[Instances].[Products]";
        private readonly ISqlExecutor _sqlExecutor;

        public ProductService(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task<int> CreateProduct(Product product)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                product.CreatedTimestamp = DateTime.Now;
                var instanceId = await connection.InsertAsync(product, transaction);
                
                return instanceId;
            });
        }

         public async Task<List<Product>> GetProducts()
         {
            string sql = "SELECT * FROM " + TableName;

            var products = await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var result = await conn.QueryAsync<Product>(sql: sql, transaction: trans);
                return result;
            });

            // Convert the query result to a list and return
            return products.ToList();

        }

        public Task<List<Product>> SearchProducts(string keyword)
        {
            throw new NotImplementedException();
        }
    }
}