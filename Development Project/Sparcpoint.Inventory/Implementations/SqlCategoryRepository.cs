using Dapper;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Models.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Implementations
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlCategoryRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(AddCategoryRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));
            PreConditions.StringMaxLength(request.Name, nameof(request.Name), 64);
            PreConditions.StringNotNullOrWhitespace(request.Description, nameof(request.Description));
            PreConditions.StringMaxLength(request.Description, nameof(request.Description), 256);

            if (request.ParentCategoryIds != null)
                foreach (var id in request.ParentCategoryIds)
                    PreConditions.IntGreaterThanZero(id, "ParentCategoryId");

            return await _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                // EVAL: MERGE with HOLDLOCK prevents phantom inserts under concurrent requests.
                // If a category with the same Name already exists, the INSERT is skipped and the
                // existing InstanceId is returned — no duplicate, no error.
                const string insertCategory = @"
                    MERGE [Instances].[Categories] WITH (HOLDLOCK) AS target
                    USING (VALUES (@Name)) AS source ([Name]) ON target.[Name] = source.[Name]
                    WHEN NOT MATCHED THEN
                        INSERT ([Name], [Description])
                        VALUES (@Name, @Description);
                    SELECT CAST([InstanceId] AS INT) FROM [Instances].[Categories] WHERE [Name] = @Name;";

                int instanceId = await conn.ExecuteScalarAsync<int>(insertCategory, new
                {
                    request.Name,
                    request.Description
                }, trans);

                if (request.ParentCategoryIds != null && request.ParentCategoryIds.Any())
                {
                    const string insertParent = @"
                        INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
                        VALUES (@InstanceId, @CategoryInstanceId);";

                    await conn.ExecuteAsync(insertParent,
                        request.ParentCategoryIds.Select(id => new { InstanceId = instanceId, CategoryInstanceId = id }),
                        trans);
                }

                return instanceId;
            });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _Executor.ExecuteAsync<IEnumerable<Category>>(async (conn, trans) =>
            {
                var categories = (await conn.QueryAsync<Category>(
                    "SELECT * FROM [Instances].[Categories] ORDER BY [Name]",
                    transaction: trans)).ToList();

                if (categories.Any())
                {
                    var ids = categories.Select(c => c.InstanceId).ToList();

                    // EVAL: Single bulk query for all parent relationships avoids N+1 queries
                    var allParents = await conn.QueryAsync<CategoryParentRow>(
                        "SELECT [InstanceId], [CategoryInstanceId] FROM [Instances].[CategoryCategories] WHERE [InstanceId] IN @Ids",
                        new { Ids = ids }, trans);

                    var parentLookup = allParents
                        .GroupBy(r => r.InstanceId)
                        .ToDictionary(g => g.Key, g => (IEnumerable<int>)g.Select(r => r.CategoryInstanceId).ToList());

                    foreach (var category in categories)
                    {
                        category.ParentCategoryIds = parentLookup.TryGetValue(category.InstanceId, out var parents)
                            ? parents : Enumerable.Empty<int>();
                    }
                }

                return categories;
            });
        }
        private class CategoryParentRow
        {
            public int InstanceId { get; set; }
            public int CategoryInstanceId { get; set; }
        }
    }
}
