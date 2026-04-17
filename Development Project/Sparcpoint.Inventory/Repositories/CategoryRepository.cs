using Dapper;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public CategoryRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public Task<int> CreateAsync(CreateCategoryRequest request)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var instanceId = await connection.ExecuteScalarAsync<int>(@"
INSERT INTO [Instances].[Categories] ([Name], [Description])
OUTPUT INSERTED.[InstanceId]
VALUES (@Name, @Description);",
                    new
                    {
                        request.Name,
                        request.Description,
                    }, transaction);

                foreach (var attribute in request.Attributes ?? new Dictionary<string, string>())
                {
                    await connection.ExecuteAsync(@"
INSERT INTO [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value])
VALUES (@InstanceId, @Key, @Value);",
                        new
                        {
                            InstanceId = instanceId,
                            Key = attribute.Key,
                            Value = attribute.Value,
                        }, transaction);
                }

                foreach (var parentCategoryId in request.ParentCategoryIds ?? new List<int>())
                {
                    await connection.ExecuteAsync(@"
INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
VALUES (@InstanceId, @CategoryInstanceId);",
                        new
                        {
                            InstanceId = instanceId,
                            CategoryInstanceId = parentCategoryId,
                        }, transaction);
                }

                return instanceId;
            });
        }

        public Task<CategoryDetailModel> GetByIdAsync(int instanceId)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                using (var grid = await connection.QueryMultipleAsync(@"
SELECT
    c.[InstanceId],
    c.[Name],
    c.[Description],
    c.[CreatedTimestamp]
FROM [Instances].[Categories] c
WHERE c.[InstanceId] = @InstanceId;

SELECT
    ca.[Key],
    ca.[Value]
FROM [Instances].[CategoryAttributes] ca
WHERE ca.[InstanceId] = @InstanceId;

SELECT
    cc.[CategoryInstanceId]
FROM [Instances].[CategoryCategories] cc
WHERE cc.[InstanceId] = @InstanceId;",
                    new { InstanceId = instanceId }, transaction))
                {
                    var row = await grid.ReadSingleOrDefaultAsync<CategoryModel>();

                    if (row == null)
                        return null;

                    var attributes = (await grid.ReadAsync<CategoryAttributeRow>())
                        .ToDictionary(item => item.Key, item => item.Value);

                    var parentCategoryIds = (await grid.ReadAsync<int>()).ToList();

                    return new CategoryDetailModel
                    {
                        InstanceId = row.InstanceId,
                        Name = row.Name,
                        Description = row.Description,
                        CreatedTimestamp = row.CreatedTimestamp,
                        Attributes = attributes,
                        ParentCategoryIds = parentCategoryIds,
                    };
                }
            });
        }

        public Task<PaginatedResult<CategoryModel>> GetAllAsync(int page, int pageSize)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                using (var grid = await connection.QueryMultipleAsync(@"
SELECT COUNT(*)
FROM [Instances].[Categories];

SELECT
    c.[InstanceId],
    c.[Name],
    c.[Description],
    c.[CreatedTimestamp]
FROM [Instances].[Categories] c
ORDER BY c.[CreatedTimestamp] ASC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;",
                    new
                    {
                        Offset = (page - 1) * pageSize,
                        PageSize = pageSize,
                    }, transaction))
                {
                    var totalCount = await grid.ReadSingleAsync<int>();
                    var items = (await grid.ReadAsync<CategoryModel>()).ToList();

                    return new PaginatedResult<CategoryModel>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        Page = page,
                        PageSize = pageSize,
                    };
                }
            });
        }

        public Task AddAttributeAsync(int instanceId, string key, string value)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                await connection.ExecuteAsync(@"
MERGE [Instances].[CategoryAttributes] AS target
USING (SELECT @InstanceId AS [InstanceId], @Key AS [Key]) AS source
ON target.[InstanceId] = source.[InstanceId] AND target.[Key] = source.[Key]
WHEN MATCHED THEN
    UPDATE SET [Value] = @Value
WHEN NOT MATCHED THEN
    INSERT ([InstanceId], [Key], [Value])
    VALUES (@InstanceId, @Key, @Value);",
                    new
                    {
                        InstanceId = instanceId,
                        Key = key,
                        Value = value,
                    }, transaction);

                return 0;
            });
        }

        public Task RemoveAttributeAsync(int instanceId, string key)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                await connection.ExecuteAsync(@"
DELETE FROM [Instances].[CategoryAttributes]
WHERE [InstanceId] = @InstanceId AND [Key] = @Key;",
                    new
                    {
                        InstanceId = instanceId,
                        Key = key,
                    }, transaction);

                return 0;
            });
        }

        public Task AddParentCategoryAsync(int instanceId, int parentCategoryId)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                await connection.ExecuteAsync(@"
IF NOT EXISTS (
    SELECT 1 FROM [Instances].[CategoryCategories]
    WHERE [InstanceId] = @InstanceId AND [CategoryInstanceId] = @CategoryInstanceId
)
INSERT INTO [Instances].[CategoryCategories] ([InstanceId], [CategoryInstanceId])
VALUES (@InstanceId, @CategoryInstanceId);",
                    new
                    {
                        InstanceId = instanceId,
                        CategoryInstanceId = parentCategoryId,
                    }, transaction);

                return 0;
            });
        }

        public Task RemoveParentCategoryAsync(int instanceId, int parentCategoryId)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                await connection.ExecuteAsync(@"
DELETE FROM [Instances].[CategoryCategories]
WHERE [InstanceId] = @InstanceId AND [CategoryInstanceId] = @CategoryInstanceId;",
                    new
                    {
                        InstanceId = instanceId,
                        CategoryInstanceId = parentCategoryId,
                    }, transaction);

                return 0;
            });
        }

        private class CategoryAttributeRow
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
