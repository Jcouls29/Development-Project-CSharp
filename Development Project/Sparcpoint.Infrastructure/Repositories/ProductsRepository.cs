using Sparcpoint.Interfaces;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ISqlExecutor sqlExecutor;

        public ProductsRepository(ISqlExecutor sqlExecutor)
        {
            this.sqlExecutor = sqlExecutor;
        }
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            Func<IDbConnection, IDbTransaction, Task<IEnumerable<Product>>> getProductsPredicate = async (conn, tran) =>
            {
                string sql = @"SELECT 
	                             [InstanceId]
                                ,[Name]
                                ,[Description]
                                ,[ProductImageUris]
                                ,[ValidSkus]
                                ,[CreatedTimestamp]
                               FROM [Sparcpoint.Inventory.Database].[Instances].[Products]";

                SqlCommand command = new SqlCommand
                {
                    Connection = (SqlConnection)conn,
                    CommandText = sql
                };

                var products = new List<Product>();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // EVAL: retrieving only basic info for a product list page. Other fields would be returned when a users views product details
                            products.Add(new Product
                            {
                                InstanceId = Convert.ToInt32(reader["InstanceId"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString()
                            });
                        }
                    }

                    reader.Close();
                }

                return products;
            };

            return await sqlExecutor.ExecuteAsync(getProductsPredicate);
        }
    }
}
