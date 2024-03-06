using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
using Dapper;
using System.Linq;

namespace Interview.Web.Services
{
     public class ProductService
     {
          private readonly ISqlExecutor _sqlExecutor;

          public ProductService(ISqlExecutor sqlExecutor)
          {
               _sqlExecutor = sqlExecutor;
          }

          public async Task<List<Product>> GetAllProductsAsync()
          {
               const string SQL_SELECT_ALL = @"SELECT * FROM inventory.Instances.Products;";
               var products = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
               {
                    var result = await conn.QueryAsync<Product>(sql: SQL_SELECT_ALL, transaction: trnx);
                    return result;
               });
               return products.ToList();
          }
     }
}
