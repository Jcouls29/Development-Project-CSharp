using Dapper;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.SqlServer
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlCategoryRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(CreateCategoryRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));
            PreConditions.StringNotNullOrWhitespace(request.Description, nameof(request.Description));

            return await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                var categoryId = await conn.ExecuteScalarAsync<int>(@"
                    INSERT INTO [Instances].[Categories] ([Name], [Description])
                    VALUES (@Name, @Description);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new { request.Name, request.Description }, tx);

                // Link to parent categories for hierarchy support
                if (request.ParentCategoryIds != null)
                {
                    foreach (var parentId in request.ParentCategoryIds)
                    {
                        await conn.ExecuteAsync(@"
                            INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
                            VALUES (@InstanceId, @CategoryInstanceId)",
                            new { InstanceId = categoryId, CategoryInstanceId = parentId }, tx);
                    }
                }

                if (request.Attributes != null)
                {
                    foreach (var attr in request.Attributes)
                    {
                        await conn.ExecuteAsync(@"
                            INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
                            VALUES (@InstanceId, @Key, @Value)",
                            new { InstanceId = categoryId, attr.Key, attr.Value }, tx);
                    }
                }

                return categoryId;
            });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                var categories = (await conn.QueryAsync<Category>(@"
                    SELECT [InstanceId], [Name], [Description], [CreatedTimestamp]
                    FROM [Instances].[Categories]
                    ORDER BY [Name]", null, tx)).ToList();

                if (categories.Any())
                {
                    var ids = categories.Select(c => c.InstanceId).ToArray();
                    var parentLinks = await conn.QueryAsync<(int InstanceId, int CategoryInstanceId)>(
                        "SELECT [InstanceId], [CategoryInstanceId] FROM [Instances].[CategoryCategories] WHERE [InstanceId] IN @Ids",
                        new { Ids = ids }, tx);

                    var parentLookup = parentLinks
                        .GroupBy(p => p.InstanceId)
                        .ToDictionary(g => g.Key, g => g.Select(p => p.CategoryInstanceId).ToList());

                    foreach (var cat in categories)
                    {
                        cat.ParentCategoryIds = parentLookup.TryGetValue(cat.InstanceId, out var parents)
                            ? parents : new List<int>();
                    }
                }

                return categories;
            });
        }
    }
}
