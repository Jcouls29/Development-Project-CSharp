using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Core.Repositories.Implementations
{
    // EVAL: Repository Implementation - Uses raw ADO.NET for database operations
    // EVAL: Dependency Injection - Constructor injection for testability
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public CategoryRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<Category> GetByIdAsync(int instanceId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = "SELECT InstanceId, Name, Description, CreatedTimestamp FROM Instances.Categories WHERE InstanceId = @InstanceId";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@InstanceId", instanceId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var category = Category.Load(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetDateTime(3));
                            return category;
                        }
                    }
                }

                return null;
            });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = "SELECT InstanceId, Name, Description, CreatedTimestamp FROM Instances.Categories ORDER BY Name";
                var categories = new List<Category>();

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var category = Category.Load(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetDateTime(3));
                            categories.Add(category);
                        }
                    }
                }

                return categories;
            });
        }

        public async Task<int> AddAsync(Category category)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    INSERT INTO Instances.Categories (Name, Description, CreatedTimestamp)
                    OUTPUT INSERTED.InstanceId
                    VALUES (@Name, @Description, @CreatedTimestamp)";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@Name", category.Name);
                    cmd.Parameters.AddWithValue("@Description", category.Description);
                    cmd.Parameters.AddWithValue("@CreatedTimestamp", category.CreatedTimestamp);

                    var categoryId = (int)await cmd.ExecuteScalarAsync();
                    category.SetInstanceId(categoryId);
                    return categoryId;
                }
            });
        }

        public async Task<IEnumerable<int>> GetParentCategoryIdsAsync(int categoryInstanceId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT CategoryInstanceId
                    FROM Instances.CategoryCategories
                    WHERE InstanceId = @InstanceId";

                var parentCategoryIds = new List<int>();
                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@InstanceId", categoryInstanceId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            parentCategoryIds.Add(reader.GetInt32(0));
                        }
                    }
                }

                return parentCategoryIds;
            });
        }

        public async Task<IEnumerable<int>> GetChildCategoryIdsAsync(int categoryInstanceId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT InstanceId
                    FROM Instances.CategoryCategories
                    WHERE CategoryInstanceId = @CategoryInstanceId";

                var childCategoryIds = new List<int>();
                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@CategoryInstanceId", categoryInstanceId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            childCategoryIds.Add(reader.GetInt32(0));
                        }
                    }
                }

                return childCategoryIds;
            });
        }

        public async Task UpdateParentCategoryRelationshipsAsync(int categoryInstanceId, IEnumerable<int> parentCategoryIds)
        {
            await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM Instances.CategoryCategories
                        WHERE InstanceId = @InstanceId";
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@InstanceId", categoryInstanceId);
                    await cmd.ExecuteNonQueryAsync();
                }

                if (parentCategoryIds != null)
                {
                    foreach (var parentCategoryId in parentCategoryIds)
                    {
                        using (var cmd = (DbCommand)conn.CreateCommand())
                        {
                            cmd.CommandText = @"
                                INSERT INTO Instances.CategoryCategories (InstanceId, CategoryInstanceId)
                                VALUES (@InstanceId, @CategoryInstanceId)";
                            cmd.Transaction = (DbTransaction)trans;
                            cmd.Parameters.AddWithValue("@InstanceId", categoryInstanceId);
                            cmd.Parameters.AddWithValue("@CategoryInstanceId", parentCategoryId);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            });
        }

        public async Task UpdateAsync(Category category)
        {
            await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    UPDATE Instances.Categories
                    SET Name = @Name, Description = @Description
                    WHERE InstanceId = @InstanceId";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@InstanceId", category.InstanceId);
                    cmd.Parameters.AddWithValue("@Name", category.Name);
                    cmd.Parameters.AddWithValue("@Description", category.Description);
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

        public async Task DeleteAsync(int instanceId)
        {
            await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Instances.Categories WHERE InstanceId = @InstanceId";
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@InstanceId", instanceId);
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }
    }
}
