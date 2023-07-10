using Dapper;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection dbConnection;
        private const string InsertProductSql = "INSERT INTO Products (Name, Description) VALUES (@Name, @Description)";
        private const string UpdateProductSql = "UPDATE Products SET Name = @Name, Description = @Description WHERE ProductId = @ProductId";
        private const string DeleteProductSql = "DELETE FROM Products WHERE ProductId = @ProductId";
        private const string SelectAllProductsSql = "SELECT * FROM Products";
        private const string SelectProductByIdSql = "SELECT * FROM Products WHERE ProductId = @ProductId";

        public ProductRepository(IDbConnection dbConnection) 
        {
            this.dbConnection = dbConnection;
        }

        //TODO: create the IServiceCollection 
        //Need to add services.AddScoped<IProductRepository, ProductRepository>(); in it
        //Would then do dependency injection to resolve IProductRepository wherever it's needed
        //Make a private readonly variable called IProductRepository productRepository
        //In ProductController and its constructor, pass IProductRepository to the constructor
        //Would be this.productRepository = productRepository;


        public async Task AddAsync(Product product, IDbTransaction transaction)
        {
            await dbConnection.ExecuteAsync(InsertProductSql, product, transaction);
        }

        public async Task DeleteAsync(int id, IDbTransaction transaction)
        {
            var parameterId = new { ProductId = id };
            await dbConnection.ExecuteAsync(DeleteProductSql, parameterId, transaction);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(IDbTransaction transaction)
        {
            return await dbConnection.QueryAsync<Product>(SelectAllProductsSql, transaction);
        }

        public async Task<Product> GetByIdAsync(int id, IDbTransaction transaction)
        {
            var parameterId = new { ProductId = id };
            return await dbConnection.QuerySingleOrDefaultAsync<Product>(SelectProductByIdSql, parameterId, transaction);
        }

        public async Task UpdateAsync(Product product, IDbTransaction transaction)
        {
            await dbConnection.ExecuteAsync(UpdateProductSql, product, transaction);
        }
    }
}
