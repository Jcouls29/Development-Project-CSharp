using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
using Dapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Services
{
     public class ProductService
     {
          private readonly ISqlExecutor _sqlExecutor;

          public ProductService(ISqlExecutor sqlExecutor)
          {
               _sqlExecutor = sqlExecutor;
          }

          //Gets all Products
          public async Task<List<Product>> GetAllProductsAsync()
          {
               // had to use DB name in SQL queries due to system config
               const string SQL_SELECT_ALL = @"SELECT * FROM inventory.Instances.Products;";
               var products = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
               {
                    var result = await conn.QueryAsync<Product>(sql: SQL_SELECT_ALL, transaction: trnx);
                    return result;
               });
               return products.ToList();
          }

          public async Task<Product> CreateProduct(CreateProductModel productModel)
          {
               Product newProduct = null;

               //Create new product and return its ID
               const string SQL_INSERT_AND_GET_PRODUCT = @"INSERT INTO inventory.Instances.Products (Name, Description, ProductImageUris, ValidSkus)
                                                           VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                                                           SELECT CAST(scope_identity() AS int);";
               int id = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
               {
                    //Using Dapper fill out the SQL variables and execute the SQL Query
                    int productId = await conn.ExecuteScalarAsync<int>(SQL_INSERT_AND_GET_PRODUCT, new
                    {
                         productModel.Name,
                         productModel.Description,
                         productModel.ProductImageUris,
                         productModel.ValidSkus
                    }, trnx);
                    return productId;
               });

               // in the case that an id was found and there wasn't an error creating the product
               if (id > 0)
               {
                    //find the new product in the database
                    string selectSql = "SELECT * FROM inventory.Instances.Products WHERE InstanceId = @Id;";
                    newProduct = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
                    {
                         var product = await conn.QueryFirstOrDefaultAsync<Product>(selectSql, new { Id = id }, trnx);
                         return product;
                    });
               }

               return newProduct;

          }
     }
}
