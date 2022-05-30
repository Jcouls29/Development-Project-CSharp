using Dapper;
using Interview.Web.Helpers;
using Interview.Web.Interfaces;
using Interview.Web.Models;
using Microsoft.Extensions.Configuration;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Interview.Web.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private ISqlExecutor _sqlServer;
        private Category _category;

        public CategoryRepository(IConfiguration configuration)
        {
            _sqlServer = new SqlServerExecutor(configuration.GetConnectionString(Constants.DefaultConnection));

        }
        Category ICategoryRepository.Add(Category category)
        {
            _category = category;
            var id = _sqlServer.Execute<int>(SqlServerExecutorAddCategory);

            category.InstanceId = id;
            return category;
        }

        List<Category> ICategoryRepository.GetAll()
        {
            return _sqlServer.Execute<List<Category>>(SqlServerExecutorGetCategories);
        }

        #region private methods

        private List<Category> SqlServerExecutorGetCategories(IDbConnection connection, IDbTransaction transaction)
        {
            var sql = Constants.CategoryGetAll;
            return connection.Query<Category>(sql, null, transaction).ToList();
        }

        private int SqlServerExecutorAddCategory(IDbConnection connection, IDbTransaction transaction)
        {
            var parameters = new { Name = _category.Name, Description = _category.Description };
            var sql = Constants.CategoryInsert;

            return connection.Query<int>(sql, parameters, transaction).Single();
        }

        #endregion
    }
}
