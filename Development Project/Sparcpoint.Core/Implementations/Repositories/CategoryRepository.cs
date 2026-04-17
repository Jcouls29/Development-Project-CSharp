using Sparcpoint.Interfaces;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace Sparcpoint.Implementations.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _executor;

        public CategoryRepository(ISqlExecutor executor)
        {
            _executor = executor;
        }

        public async Task<int> CreateAsync(Category category)
        {
            return await _executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                var sql = @"
                    INSERT INTO [Instances].[Categories] (Name, Description)
                    VALUES (@Name, @Description);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                return await conn.QuerySingleAsync<int>(sql, new
                {
                    category.Name,
                    category.Description
                }, trans);
            });
        }

        public async Task<Category> GetByIdAsync(int instanceId)
        {
            return await _executor.ExecuteAsync<Category>(async (conn, trans) =>
            {
                var sql = @"
                    SELECT * FROM [Instances].[Categories] 
                    WHERE InstanceId = @InstanceId";

                return await conn.QuerySingleOrDefaultAsync<Category>(
                    sql, new { InstanceId = instanceId }, trans);
            });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _executor.ExecuteAsync<IEnumerable<Category>>(async (conn, trans) =>
            {
                var sql = "SELECT * FROM [Instances].[Categories]";
                return await conn.QueryAsync<Category>(sql, transaction: trans);
            });
        }
    }
}
