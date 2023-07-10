using Dapper;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Data
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnection dbConnection;
        private const string InsertCategorySql = "INSERT INTO Categories (Name, Description) VALUES (@Name, @Description)";
        private const string UpdateCategorySql = "UPDATE Categories SET Name = @Name, Description = @Description WHERE CategoryId = @CategoryId";
        private const string DeleteCategorySql = "DELETE FROM Categories WHERE CategoryId = @CategoryId";
        private const string GetByIdCategorySql = "SELECT * FROM Categories WHERE CategoryId = @CategoryId";
        private const string GetAllCategorySql = "SELECT * FROM Categories";

        public CategoryRepository(IDbConnection dbConnection) 
        {
            this.dbConnection = dbConnection;
        }

        //TODO: create the IServiceCollection 
        //Need to add services.AddScoped<ICategoryRepository, CategoryRepository>(); in it
        //Would then do dependency injection to resolve ICategoryRepository wherever it's needed
        //Make a private readonly variable called ICategoryRepository categoryRepository
        //Make CategoryController and its constructor and pass ICategoryRepository to the constructor
        //Would be this.categoryRepository = categoryRepository;

        public async Task AddAsync(Category category, IDbTransaction transaction)
        {
            await dbConnection.ExecuteAsync(InsertCategorySql, category, transaction);
        }

        public async Task DeleteAsync(int id, IDbTransaction transaction)
        {
            var parameterId = new { CategoryId = id };
            await dbConnection.ExecuteAsync(DeleteCategorySql, parameterId, transaction);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(IDbTransaction transaction)
        {
            return await dbConnection.QueryAsync<Category>(GetAllCategorySql, transaction);
        }

        public async Task<Category> GetByIdAsync(int id, IDbTransaction transaction)
        {
            var parameterId = new { CategoryId = id };
            return await dbConnection.QuerySingleOrDefaultAsync<Category>(GetByIdCategorySql, parameterId, transaction);
        }

        public async Task UpdateAsync(Category category, IDbTransaction transaction)
        {
            await dbConnection.ExecuteAsync(UpdateCategorySql, category, transaction);
        }
    }
}
