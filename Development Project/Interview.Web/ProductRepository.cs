using Dapper;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Repositories.Interfaces;

namespace Interview.Web.Repositories
{
    //EVAL
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _db;

        public ProductRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<Product> GetProductById(int id)
        {
            string query = "SELECT * FROM Products WHERE ProductID = @ProductID";
            return await _db.QuerySingleOrDefaultAsync<Product>(query, new { ProductID = id });
        }

        public async Task<Product> CreateProduct(Product product)
        {
            var sql = @"INSERT INTO Products (Name, SKU, Brand, ProductImageUris, Description, Color, Quantity, CreatedTimestamp) 
                        VALUES (@Name, @SKU, @Brand, @ProductImageUris, @Description, @Color, @Quantity, @CreatedTimestamp)
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = await _db.ExecuteScalarAsync<int>(sql, product);

            product.ProductID = id;
            return product;
        }

        public async Task AddProduct(Product product)
        {
            var sql =
                @"INSERT INTO Products (Name, SKU, Brand, ProductImageUris, Description, Color, Quantity, CreatedTimestamp) 
                        VALUES (@Name, @SKU, @Brand, @ProductImageUris, @Description, @Color, @Quantity, @CreatedTimestamp);";

            try
            {
                await _sqlExecutor.Connection.ExecuteAsync(sql, product);
            }
            catch (SqlException ex) when
                (ex.Number == 2627) // 2627 is SQL Server's error code for unique constraint violation
            {
                throw new Exception("A product with this SKU or Name already exists.", ex);
            }
        }


        public async Task<IEnumerable<Product>> SearchProducts(string category, string name, string sku, string brand, string color)
        {
            var sql = @"SELECT * FROM Products WHERE ((@Name is null or Name = @Name) 
                        AND (@SKU is null or SKU = @SKU) 
                        AND (@Brand is null or Brand = @Brand)
                        AND (@Color is null or Color = @Color));";

            var result = await _db.QueryAsync<Product>(sql, new { Name = name, SKU = sku, Brand = brand, Color = color });
            return result.ToList();
        }
    }
}
