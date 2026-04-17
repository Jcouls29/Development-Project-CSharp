using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Interview.DataEntities.Models;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ISqlExecutor _SqlExecutor;

        public CategoryService(ISqlExecutor sqlExecutor)
        {
            _SqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<int> CreateCategoryAsync(CategoryRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Category name is required.", nameof(request.Name));

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Category description is required.", nameof(request.Description));

            return await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                int categoryId;
                const string sql = @"
                    INSERT INTO [Instances].[Categories] (Name, Description)
                    VALUES (@Name, @Description);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddWithValue("@Name", request.Name);
                    cmd.Parameters.AddWithValue("@Description", request.Description);
                    categoryId = (int)await cmd.ExecuteScalarAsync();
                }

                if (request.ParentCategoryId.HasValue)
                {
                    const string hierarchySql = @"
                        INSERT INTO [Instances].[CategoryCategories] (InstanceId, CategoryId)
                        VALUES (@ParentId, @ChildId)";
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        cmd.CommandText = hierarchySql;
                        cmd.Transaction = (SqlTransaction)transaction;
                        cmd.Parameters.AddWithValue("@ParentId", request.ParentCategoryId.Value);
                        cmd.Parameters.AddWithValue("@ChildId", categoryId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return categoryId;
            });
        }

        public async Task AddProductToCategoryAsync(int productId, int categoryId)
        {
            await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                const string sql = "INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryId) VALUES (@ProductId, @CategoryId)";
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

        public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync()
        {
            return await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var categories = new List<CategoryResponse>();
                const string sql = "SELECT InstanceId, Name, Description, CreatedTimestamp FROM [Instances].[Categories]";
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categories.Add(new CategoryResponse
                            {
                                InstanceId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                CreatedTimestamp = reader.GetDateTime(3)
                            });
                        }
                    }
                }
                return categories;
            });
        }

        public async Task<IEnumerable<CategoryAttributeResponse>> GetCategoryAttributesAsync(int categoryId)
        {
            return await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var attributes = new List<CategoryAttributeResponse>();
                const string sql = "SELECT [Key], [Value] FROM [Instances].[CategoryAttributes] WHERE [InstanceId] = @CategoryId";
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            attributes.Add(new CategoryAttributeResponse
                            {
                                Key = reader.GetString(0),
                                Value = reader.GetString(1)
                            });
                        }
                    }
                }
                return attributes;
            });
        }
    }
}
